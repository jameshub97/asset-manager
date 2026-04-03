import './assets/main.css'
import 'vue-toastification/dist/index.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Toast, { POSITION, type PluginOptions } from 'vue-toastification'

import App from './App.vue'
import router from './router'

const app = createApp(App)
const toastOptions: PluginOptions = {
	position: POSITION.TOP_RIGHT,
	timeout: 3000,
	hideProgressBar: false,
	closeOnClick: true,
	pauseOnHover: true,
}

app.use(createPinia())
app.use(router)
app.use(Toast, toastOptions)

app.mount('#app')
