/// <reference types="vite/client" />
/// <reference types="unplugin-vue-router/client" />

// Allow importing '.vue' files in TypeScript
declare module '*.vue' {
	import type { DefineComponent } from 'vue'
	const component: DefineComponent<{}, {}, any>
	export default component
}