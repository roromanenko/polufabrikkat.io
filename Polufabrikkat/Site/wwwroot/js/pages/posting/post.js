const app = Vue.createApp({
	data() {
		return {
			refreshTikTokPostStatusUrl: refreshTikTokPostStatusUrl,
			post: this.initializePost(post),
		};
	},
	computed: {
	},
	methods: {
		initializePost(post) {
			post.created = new Date(post.created).toLocaleString();
			post.scheduledPublicationTime = post.scheduledPublicationTime ? new Date(post.scheduledPublicationTime).toLocaleString() : null
			return post;
		},
		async refreshTikTokPostStatus() {
			try {
				const response = await fetch(refreshTikTokPostStatusUrl, {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json',
					},
					body: JSON.stringify({
						postId: this.post.id,
						publicationId: this.post.tikTokPublishId,
						tikTokUserUnionId: this.post.tikTokUserUnionId
					}),
				});

				if (response.ok) {
					const content = await response.json();
					this.post.tikTokPostStatus = content;
				}
			}
			catch (error) {
				console.log(error);
			}
		},
	}
});

app.mount('#post-app');
