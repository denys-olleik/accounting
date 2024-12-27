const ProductsAndServicesSearch = {
  name: 'ProductsAndServicesSearch',
  props: ['productsOrServices', 'selectedCustomer'],
  data() {
    return {
      searchQuery: '',
      selectedIndex: 0,
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
    },
    handleKeyDown(event) {
      if (event.key === 'ArrowDown') {
        this.selectedIndex = (this.selectedIndex + 1) % this.filteredProductsOrServices.length;
        event.preventDefault();
      } else if (event.key === 'ArrowUp') {
        this.selectedIndex = (this.selectedIndex - 1 + this.filteredProductsOrServices.length) % this.filteredProductsOrServices.length;
        event.preventDefault();
      } else if (event.key === 'Enter') {
        event.preventDefault();
        if (this.searchQuery.trim() === '') {
          this.submitForm();
        } else {
          this.selectProductOrService(this.filteredProductsOrServices[this.selectedIndex]);
        }
      }
    },
    resetSelectedIndex() {
      this.selectedIndex = 0;
    },
    focusInput() {
      this.$refs.searchInput.focus();
    },
    submitForm() {
      document.querySelector('form#app').submit();
    }
  },
  watch: {
    searchQuery: 'resetSelectedIndex',
    selectedCustomer(newValue) {
      if (newValue) {
        this.$nextTick(this.focusInput);
      }
    }
  },
  template: `
<div class="color-white max-height-250px overflow-auto margin-bottom-5px font-Roboto-Mono">
  <input type="text"
         ref="searchInput"
         class="font-size-20px width-100"
         placeholder="Search products and services"
         v-model="searchQuery"
         @focus="isSearchActive = true"
         @keydown="handleKeyDown">

  <div v-if="filteredProductsOrServices.length" class="background-color-black padding-5px box-shadow">
    <div v-for="(productOrService, index) in filteredProductsOrServices"
         :key="productOrService.id"
         class="font-size-16px"
         @click="selectProductOrService(productOrService)">
      <span v-if="index === selectedIndex" class="blinking-cursor margin-right-10px">█</span>
      <span>{{ productOrService.name }}</span>
    </div>
  </div>
</div>
  `
}