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
			imagePreviews: [],

			publishOption: '',
			scheduleTime: '',

			draggedIndex: null,
		};
	},
	computed: {
		privacyLevelsString() {
			// Join the array elements into a single string, separated by commas
			return this.selectedTikTokUser?.queryCreatorInfo?.privacyLevelOptions.join(', ');
		}
	},
	watch: {
		publishOption(newValue) {
			if (newValue === 'publishNow') {
				this.scheduleTime = null; // Reset scheduleTime when 'publishNow' is selected
			}
		}
	},
	methods: {
		initializeTikTokUsers(tiktokUsers) {
			return tiktokUsers.map(user => this.createTikTokUser(user));
		},
		createTikTokUser(tiktokUser) {
			// Extend the TikTokUser object with a remove method
			return {
				...tiktokUser,
			};
		},
		userSelected(unionId) {
			if (this.selectedTikTokUser) {
				return this.selectedTikTokUser.unionId == unionId;
			}

			return false;
		},
		async onUserChange(unionId) {
			try {
				this.selectedTikTokUser = this.tiktokUsers.find(x => x.unionId == unionId);

				const response = await fetch(`${this.selectTikTokUserUrl}?unionId=${this.selectedTikTokUser.unionId}`, {
					method: 'GET'
				});
				if (response.ok) {
					this.selectedTikTokUser.queryCreatorInfo = await response.json();
				} else {
					console.error("Failed to select TikTokUser");
				}
			} catch (error) {
				console.error("Error during API call:", error);
			} finally {
			}
		},
		handleFileChange(event) {
			for (let i = 0; i < event.target.files.length; i++) {
				const file = event.target.files[i];

				// Ensure the file is an image
				if (file.type.startsWith('image/')) {
					this.filesToUpload.push(file);
					const reader = new FileReader();
					reader.onload = (e) => {
						this.imagePreviews.push(e.target.result); // Store the image preview URL
					};

					reader.readAsDataURL(file); // Read the image file as a data URL
				}
			}
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
			formData.append('tikTokUserUnionId', this.selectedTikTokUser.unionId);
			formData.append('scheduledPublicationTime', this.scheduleTime);

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
		},
		resetTextAreaHeight() {
			this.$refs.textarea.style.height = '100px';
		},
		// Triggered when dragging starts, store the dragged image index
		dragStart(index) {
			this.draggedIndex = index;
		},

		// Triggered when an image is dropped
		drop(index) {
			if (this.draggedIndex !== null) {
				// Swap the images
				const draggedImage = this.imagePreviews[this.draggedIndex];
				this.imagePreviews.splice(this.draggedIndex, 1); // Remove dragged image
				this.imagePreviews.splice(index, 0, draggedImage); // Insert it at the new position

				const file = this.filesToUpload[this.draggedIndex];
				this.filesToUpload.splice(this.draggedIndex, 1); // Remove dragged image
				this.filesToUpload.splice(index, 0, file); // Insert it at the new position
			}
			this.draggedIndex = null; // Reset dragged index
		},

		removeImage(index) {
			this.imagePreviews.splice(index, 1); // Remove image at the specified index
			this.filesToUpload.splice(index, 1); // Remove image at the specified index
		}
	}
});

app.mount('#posting-app');