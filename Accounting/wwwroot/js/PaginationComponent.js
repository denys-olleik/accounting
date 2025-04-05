const PaginationComponent = {
	name: 'PaginationComponent',
	props: {
		prevPageUrl: {
			type: String,
			required: false
		},
		nextPageUrl: {
			type: String,
			required: false
		},
		isFiltered: {
			type: Boolean,
			required: true
		},
		page: {
			type: Number,
			required: true
		}
	},
	template: `
		<div v-if="(prevPageUrl || nextPageUrl) && !isFiltered && (page > 1 || nextPageUrl)"
				 class="margin-bottom-5px display-flex font-Roboto font-size-16px background-color-antiquewhite">
			<div>
				<a :href="prevPageUrl" v-if="page > 1" class="color-blue">prev</a>
			</div>
			<div class="flex-1"></div>
			<div>
				<a :href="nextPageUrl" v-if="nextPageUrl" class="color-blue">next</a>
			</div>
		</div>
	`
};