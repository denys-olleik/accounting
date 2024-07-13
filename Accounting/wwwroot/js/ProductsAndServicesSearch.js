const ProductsAndServicesSearch = {
  name: 'ProductsAndServicesSearch',
  props: ['productsOrServices'],
  data() {
    return {
      searchQuery: '',
    };
  },
  computed: {
    filteredProductsOrServices() {
      if (this.searchQuery) {
        return this.productsOrServices.filter(productOrService =>
          productOrService.name.toLowerCase().includes(this.searchQuery.toLowerCase())
        );
      } else {
        return this.productsOrServices;
      }
    }
  },
  methods: {
    selectProductOrService(productOrService) {
      this.$emit('product-or-service-selected', productOrService);
      this.searchQuery = '';
    }
  },
  template: `
    <div>
    <input type="text"
           class="font-size-20px width-100"
           placeholder="Search products and services"
           v-model="searchQuery"
           @focus="isSearchActive = true">

    <div v-if="searchQuery" v-for="productOrService in filteredProductsOrServices"
         :key="productOrService.id"
         class="dropdown-item font-size-16px"
         @click="selectProductOrService(productOrService)">
      {{ productOrService.name }}
    </div>
  </div>`
}