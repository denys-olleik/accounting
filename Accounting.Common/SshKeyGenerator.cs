//MIT License

//Copyright (c) 2022 Shaw Innes

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

//https://github.com/ShawInnes/SshKeyGenerator

using System.Security.Cryptography;
using System.Text;

namespace SshKeyGenerator
{
  public class SshKeyGenerator : IDisposable
  {
    private RSACryptoServiceProvider csp;

    public SshKeyGenerator(int keySize)
    {
      this.csp = new RSACryptoServiceProvider(keySize);
    }

    /// <summary>
    /// Returns the private key in x509 format
    /// </summary>
    /// <returns>string containing x509 encoded private key</returns>
    public string ToPrivateKey()
    {
      if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", nameof(csp));
      var parameters = csp.ExportParameters(true);
      using (var stream = new MemoryStream())
      {
        var writer = new BinaryWriter(stream);
        writer.Write((byte)0x30); // SEQUENCE
        using (var innerStream = new MemoryStream())
        {
          var innerWriter = new BinaryWriter(innerStream);
          EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
          EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
          EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
          EncodeIntegerBigEndian(innerWriter, parameters.D);
          EncodeIntegerBigEndian(innerWriter, parameters.P);
          EncodeIntegerBigEndian(innerWriter, parameters.Q);
          EncodeIntegerBigEndian(innerWriter, parameters.DP);
          EncodeIntegerBigEndian(innerWriter, parameters.DQ);
          EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
          var length = (int)innerStream.Length;
          EncodeLength(writer, length);
          writer.Write(innerStream.GetBuffer(), 0, length);
        }

        var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
        using (StringWriter outputStream = new StringWriter())
        {
          outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
          // Output as Base64 with lines chopped at 64 characters
          for (var i = 0; i < base64.Length; i += 64)
          {
            outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
          }

          outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
          return outputStream.ToString();
        }
      }
    }

    public string ToRfcPublicKey() => ToRfcPublicKey("generated-key");

    /// <summary>
    /// Export blobs to save or import directly into another CSP object.
    /// </summary>
    /// <param name="includePrivateKey">Include private key or not</param>
    /// <returns>CSP Blobs</returns>
    public byte[] ToBlobs(bool includePrivateKey)
    {
      return csp.ExportCspBlob(includePrivateKey);
    }

    /// <summary>
    /// Export blobs in base64 format to save or import directly into another CSP object
    /// </summary>
    /// <param name="includePrivateKey">Include private key or not</param>
    /// <returns>Base64 encoded blobs</returns>
    public string ToB64Blob(bool includePrivateKey)
    {
      return Convert.ToBase64String(ToBlobs(includePrivateKey));
    }

    /// <summary>
    /// Export Csp as XML string. This XML contains both private key and public key (P and Q).
    /// </summary>
    /// <returns>XML string</returns>
    public string ToXml()
    {
      return csp.ToXmlString(true);
    }

    /// <summary>
    /// Returns the SSH public key in RFC4716 format
    /// </summary>
    /// <returns>string containing RFC4716 public key</returns>
    public string ToRfcPublicKey(string comment)
    {
      byte[] sshrsaBytes = Encoding.Default.GetBytes("ssh-rsa");
      byte[] n = csp.ExportParameters(false).Modulus;
      byte[] e = csp.ExportParameters(false).Exponent;
      string buffer64;
      using (MemoryStream ms = new MemoryStream())
      {
        ms.Write(ToBytes(sshrsaBytes.Length), 0, 4);
        ms.Write(sshrsaBytes, 0, sshrsaBytes.Length);
        ms.Write(ToBytes(e.Length), 0, 4);
        ms.Write(e, 0, e.Length);
        ms.Write(ToBytes(n.Length + 1), 0, 4); //Remove the +1 if not Emulating Putty Gen
        ms.Write(new byte[] { 0 }, 0, 1); //Add a 0 to Emulate PuttyGen
        ms.Write(n, 0, n.Length);
        ms.Flush();
        buffer64 = Convert.ToBase64String(ms.ToArray());
      }

      return $"ssh-rsa {buffer64} {comment}";
    }

    /// <summary>
    /// Returns the SSH public key in x509 format
    /// </summary>
    /// <returns>string containing x509 encoded public key</returns>
    public string ToPublicKey()
    {
      var parameters = csp.ExportParameters(false);
      using (var stream = new MemoryStream())
      {
        var writer = new BinaryWriter(stream);
        writer.Write((byte)0x30); // SEQUENCE
        using (var innerStream = new MemoryStream())
        {
          var innerWriter = new BinaryWriter(innerStream);
          innerWriter.Write((byte)0x30); // SEQUENCE
          EncodeLength(innerWriter, 13);
          innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
          var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
          EncodeLength(innerWriter, rsaEncryptionOid.Length);
          innerWriter.Write(rsaEncryptionOid);
          innerWriter.Write((byte)0x05); // NULL
          EncodeLength(innerWriter, 0);
          innerWriter.Write((byte)0x03); // BIT STRING
          using (var bitStringStream = new MemoryStream())
          {
            var bitStringWriter = new BinaryWriter(bitStringStream);
            bitStringWriter.Write((byte)0x00); // # of unused bits
            bitStringWriter.Write((byte)0x30); // SEQUENCE
            using (var paramsStream = new MemoryStream())
            {
              var paramsWriter = new BinaryWriter(paramsStream);
              EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
              EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
              var paramsLength = (int)paramsStream.Length;
              EncodeLength(bitStringWriter, paramsLength);
              bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
            }

            var bitStringLength = (int)bitStringStream.Length;
            EncodeLength(innerWriter, bitStringLength);
            innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
          }

          var length = (int)innerStream.Length;
          EncodeLength(writer, length);
          writer.Write(innerStream.GetBuffer(), 0, length);
        }

        var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
        using (StringWriter outputStream = new StringWriter())
        {
          outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
          for (var i = 0; i < base64.Length; i += 64)
          {
            outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
          }

          outputStream.WriteLine("-----END PUBLIC KEY-----");

          return outputStream.ToString();
        }
      }
    }

    private void EncodeLength(BinaryWriter stream, int length)
    {
      if (length < 0) throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative");
      if (length < 0x80)
      {
        // Short form
        stream.Write((byte)length);
      }
      else
      {
        // Long form
        var temp = length;
        var bytesRequired = 0;
        while (temp > 0)
        {
          temp >>= 8;
          bytesRequired++;
        }

        stream.Write((byte)(bytesRequired | 0x80));
        for (var i = bytesRequired - 1; i >= 0; i--)
        {
          stream.Write((byte)(length >> (8 * i) & 0xff));
        }
      }
    }

    private void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
    {
      stream.Write((byte)0x02); // INTEGER
      var prefixZeros = 0;
      for (var i = 0; i < value.Length; i++)
      {
        if (value[i] != 0) break;
        prefixZeros++;
      }

      if (value.Length - prefixZeros == 0)
      {
        EncodeLength(stream, 1);
        stream.Write((byte)0);
      }
      else
      {
        if (forceUnsigned && value[prefixZeros] > 0x7f)
        {
          // Add a prefix zero to force unsigned if the MSB is 1
          EncodeLength(stream, value.Length - prefixZeros + 1);
          stream.Write((byte)0);
        }
        else
        {
          EncodeLength(stream, value.Length - prefixZeros);
        }

        for (var i = prefixZeros; i < value.Length; i++)
        {
          stream.Write(value[i]);
        }
      }
    }

    private byte[] ToBytes(int i)
    {
      byte[] bts = BitConverter.GetBytes(i);
      if (BitConverter.IsLittleEndian)
      {
        Array.Reverse(bts);
      }

      return bts;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.csp?.Dispose();
      }
    }

    ~SshKeyGenerator()
    {
      Dispose(false);
    }
  }
}