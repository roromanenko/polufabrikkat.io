const app = Vue.createApp({
	data() {
		return {
			tiktokUsers: this.initializeTikTokUsers(modelTikTokUsers),
			removeTikTokUserUrl: removeTikTokUserUrl,
			changePasswordUrl: changePasswordUrl,

			isPopupShows: false,
			newPassword: '',
			confirmNewPassword: '',
			newPasswordErrors: ''
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
		},
		showPasswordPopup() {
			this.isPopupShows = true;
		},
		hidePasswordPopup() {
			this.isPopupShows = false;
			this.newPassword = '';
			this.confirmNewPassword = '';
			this.newPasswordErrors = '';
		},
		async changePassword() {
			if (this.newPassword != this.confirmNewPassword) {
				this.newPasswordErrors = 'Password and confirmation should be equal';
				return;
			}

			try {
				const response = await fetch(changePasswordUrl, {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json',
					},
					body: JSON.stringify({
						newPassword: this.newPassword,  // Send only the newPassword
					}),
				});

				if (response.ok) {
					this.hidePasswordPopup();
				} else {
					const error = await response.json();
					this.newPasswordErrors = `Error: ${error.message}`;
					console.log(error);
				}
			}
			catch(error) {
				this.newPasswordErrors = 'An error occurred while changing the password.';
				console.log(error);
			}
		}
	}
});

app.mount('#tiktok-users-app');