const app = Vue.createApp({
	data() {
		return {
			tiktokUsers: this.initializeTikTokUsers(modelTikTokUsers),
			removeTikTokUserUrl: removeTikTokUserUrl
		};
	},
	methods: {
		initializeTikTokUsers(tiktokUsers) {
			return tiktokUsers.map(user => this.createTikTokUser(user));
		},
		createTikTokUser(tiktokUser) {
			// Extend the TikTokUser object with a remove method
			return {
				...tiktokUser,
				loading: false, // Add a loading state to each user
				remove: async () => {
					this.loading = true;
					try {
						const response = await fetch(`${this.removeTikTokUserUrl}?unionId=${tiktokUser.unionId}`, {
							method: 'DELETE'
						});
						if (response.ok) {
							this.tiktokUsers = this.tiktokUsers.filter(u => u.unionId !== tiktokUser.unionId);
						} else {
							console.error("Failed to remove TikTokUser");
						}
					} catch (error) {
						console.error("Error during API call:", error);
					} finally {
						this.loading = false;
					}
				}
			};
		}
	}
});

app.mount('#tiktok-users-app');