const app = Vue.createApp({
	data() {
		return {
			getAllFilesUrl: getAllFilesUrl,
			getAllFilesFromDbUrl: getAllFilesFromDbUrl,
			uploadFileUrl: uploadFileUrl,
			getFileUrl: getFileUrl,

			fileNames: [],
			fileUrlsFromDb: [],
			selectedUploadFile: null
		};
	},
	methods: {
		async getAllFiles() {
			try {
				const response = await fetch(this.getAllFilesUrl, {
					method: 'GET'
				});
				if (response.ok) {
					this.fileNames = await response.json();
				} else {
					console.error("error");
				}
			} catch (error) {
				console.error("Error during API call:", error);
			} finally {
			}
		},
		async getAllFilesFromDb() {
			try {
				const response = await fetch(this.getAllFilesFromDbUrl, {
					method: 'GET'
				});
				if (response.ok) {
					var fileNamesFromDb = await response.json();
					this.fileUrlsFromDb = fileNamesFromDb.map(x => `${getFileUrl}/${x}`)
				} else {
					console.error("error");
				}
			} catch (error) {
				console.error("Error during API call:", error);
			} finally {
			}
		},
		onUploadFileSelected(event) {
			this.selectedUploadFile = event.target.files[0]; // Store the selected file
		},
		async uploadFile() {
			if (!this.selectedUploadFile) return;

			const formData = new FormData();
			formData.append('file', this.selectedUploadFile); // Append the file to FormData

			try {
				// Use fetch to send the file
				const response = await fetch(this.uploadFileUrl, {
					method: 'POST',
					body: formData,
				});

				if (response.ok) {
					alert("File uploaded")
				} else {
					alert("Failed to upload file")
				}
			} catch (error) {
				console.error('Error uploading file:', error);
			}
		}
	}
});

app.mount('#admin-app');