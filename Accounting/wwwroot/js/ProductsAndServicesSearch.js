const ProductsAndServicesSearch = {
  name: 'ProductsAndServicesSearch',
  props: ['productsOrServices'],
  data() {
    return {
      searchQuery: '',
      selectedIndex: 0, // Track the currently selected index
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
    // Handle keyboard navigation and prevent form submission
    handleKeyDown(event) {
      if (event.key === 'ArrowDown') {
        this.selectedIndex = (this.selectedIndex + 1) % this.filteredProductsOrServices.length;
        event.preventDefault();
      } else if (event.key === 'ArrowUp') {
        this.selectedIndex = (this.selectedIndex - 1 + this.filteredProductsOrServices.length) % this.filteredProductsOrServices.length;
        event.preventDefault();
      } else if (event.key === 'Enter') {
        // Prevent default form submission
        event.preventDefault();
        this.selectProductOrService(this.filteredProductsOrServices[this.selectedIndex]);
      }
    },
    // Reset selected index when the search query changes
    resetSelectedIndex() {
      this.selectedIndex = 0;
    }
  },
  watch: {
    searchQuery: 'resetSelectedIndex'
  },
  template: `
    <div>
      <input type="text"
             class="font-size-20px width-100"
             placeholder="Search products and services"
             v-model="searchQuery"
             @focus="isSearchActive = true"
             @keydown="handleKeyDown"> <!-- Added keydown event -->

      <div v-if="searchQuery">
        <div v-for="(productOrService, index) in filteredProductsOrServices"
             :key="productOrService.id"
             class="dropdown-item font-size-16px"
             @click="selectProductOrService(productOrService)">
          <span v-if="index === selectedIndex" class="blinking-cursor">█</span> <!-- Blinking cursor -->
          {{ productOrService.name }}
        </div>
      </div>
    </div>
  `
}