const app = Vue.createApp({
	data() {
		return {
			tiktokUsers: this.initializeTikTokUsers(modelTikTokUsers),
			selectTikTokUserUrl: selectTikTokUserUrl,
			selectedTikTokUser: ''
		};
	},
	methods: {
		initializeTikTokUsers(tiktokUsers) {
			return tiktokUsers.map(user => this.createTikTokUser(user));
		},
		createTikTokUser(tiktokUser) {
			// Extend the TikTokUser object with a remove method
			return {
				...tiktokUser
			};
		},
		async onUserChange() {
			try {
				const response = await fetch(`${this.selectTikTokUserUrl}?unionId=${this.selectedTikTokUser.unionId}`, {
					method: 'GET'
				});
				if (response.ok) {
					//this.tiktokUsers = this.tiktokUsers.filter(u => u.unionId !== tiktokUser.unionId);
				} else {
					console.error("Failed to remove TikTokUser");
				}
			} catch (error) {
				console.error("Error during API call:", error);
			} finally {
			}
		}
	}
});

app.mount('#posting-app');