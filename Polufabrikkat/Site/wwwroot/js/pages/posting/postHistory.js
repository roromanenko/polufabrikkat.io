const app = Vue.createApp({
	data() {
		return {
			posts: this.initializePosts(posts),
		};
	},
	computed: {
	},
	methods: {
		initializePosts(posts) {
			return posts.map(x => {
				return {
					...x,
					created: new Date(x.created).toLocaleString(),
					scheduledPublicationTime: x.scheduledPublicationTime ? new Date(x.scheduledPublicationTime).toLocaleString() : null
				}
			})
		}
	}
});

app.mount('#post-history-app');
