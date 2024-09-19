// Create Vue.js app with dynamic components
const app = Vue.createApp({
	data() {
		return {
			currentComponent: 'Login'
		};
	},
	methods: {
		switchComponent(component) {
			this.currentComponent = component;
		}
	},
	mounted() {
		this.currentComponent = 'Login';
	}
});

// Mount the Vue.js app to the #app div
app.mount('#login');