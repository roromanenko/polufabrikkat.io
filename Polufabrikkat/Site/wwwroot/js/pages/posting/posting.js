const app = Vue.createApp({
	data() {
		return {
			tiktokUsers: this.initializeTikTokUsers(modelTikTokUsers),
			selectTikTokUserUrl: selectTikTokUserUrl,
			createPostUrl: createPostUrl,
			selectedTikTokUser: '',
			title: '',
			description: '',
			privacyLevel: '',
			disableComment: false,
			autoAddMusic: false,
			photoCoverIndex: 0,
			filesToUpload: [],

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
					this.selectedTikTokUser.queryCreatorInfo = await response.json();
				} else {
					console.error("Failed to remove TikTokUser");
				}
			} catch (error) {
				console.error("Error during API call:", error);
			} finally {
			}
		},
		handleFileChange(event) {
			this.filesToUpload = Array.from(event.target.files); // Store selected files
		},
		async uploadFiles() {
			const formData = new FormData();

			// Append each file to FormData
			this.filesToUpload.forEach((file, index) => {
				formData.append(`Files`, file);
			});

			// Append metadata
			formData.append('title', this.title);
			formData.append('description', this.description);
			formData.append('privacyLevel', this.privacyLevel);
			formData.append('disableComment', this.disableComment);
			formData.append('autoAddMusic', this.autoAddMusic);
			formData.append('photoCoverIndex', this.photoCoverIndex);

			try {
				// Replace `YOUR_API_ENDPOINT` with your .NET endpoint
				const response = await fetch(this.createPostUrl, {
					method: 'POST',
					body: formData,
				});
				const content = await response.json();
				console.log(content);
			} catch (error) {
				console.error('Upload failed:', error);
			}
		}
	}
});

app.mount('#posting-app');