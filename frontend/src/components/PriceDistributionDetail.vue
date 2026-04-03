<script setup lang="ts">
import { computed } from 'vue'
import { useAssetStore } from '@/stores/assetstore'
import type { Asset } from '@/services/api'

defineProps<{
	asset: Asset | null
}>()

const emit = defineEmits<{
	close: []
}>()

const store = useAssetStore()

const allAssets = computed(() => store.assets)

const stats = computed(() => {
	const prices = allAssets.value.map((item) => item.price || 0).filter((price) => price > 0)

	if (!prices.length) {
		return { min: 0, max: 0, median: 0, average: 0 }
	}

	const sorted = [...prices].sort((left, right) => left - right)
	const middle = Math.floor(sorted.length / 2)
	const lowerMiddle = sorted[middle - 1] ?? sorted[0] ?? 0
	const upperMiddle = sorted[middle] ?? sorted[sorted.length - 1] ?? 0
	const median = sorted.length % 2 === 0
		? (lowerMiddle + upperMiddle) / 2
		: upperMiddle

	return {
		min: sorted[0] ?? 0,
		max: sorted[sorted.length - 1] ?? 0,
		median,
		average: prices.reduce((sum, value) => sum + value, 0) / prices.length,
	}
})

const priceRange = computed(() => {
	return Math.max(stats.value.max - stats.value.min, 1)
})

const comparisonStats = computed(() => {
	const prices = store.comparisonAssets.map((item) => item.price || 0)
	if (!prices.length) {
		return { min: 0, max: 0, average: 0 }
	}

	return {
		min: Math.min(...prices),
		max: Math.max(...prices),
		average: prices.reduce((sum, value) => sum + value, 0) / prices.length,
	}
})

const getPricePosition = (price?: number) => {
	if (!price) return 0
	return ((price - stats.value.min) / priceRange.value) * 100
}

const getPricePercentile = (price?: number) => {
	if (!price) return 0

	const prices = allAssets.value.map((item) => item.price || 0).filter((value) => value > 0).sort((left, right) => left - right)
	if (!prices.length) return 0

	const lowerCount = prices.filter((value) => value < price).length
	return Math.round((lowerCount / prices.length) * 100)
}

const formatDate = (dateStr?: string) => {
	if (!dateStr) return 'N/A'

	return new Date(dateStr).toLocaleDateString('en-US', {
		year: 'numeric',
		month: 'short',
		day: 'numeric',
	})
}

const formatPrice = (value?: number) => {
	return `$${(value || 0).toFixed(2)}`
}
</script>

<template>
	<div v-if="store.comparisonAssets.length >= 2" class="comparison-chart-container">
		<div class="panel-header">
			<div>
				<h3>Price Comparison</h3>
				<p>Compare selected assets against each other.</p>
			</div>
			<button class="clear-btn" @click="store.clearComparison()">Clear</button>
		</div>

		<div class="comparison-bars">
			<div v-for="compareAsset in store.comparisonAssets" :key="compareAsset.id" class="comparison-row">
				<div class="comparison-meta">
					<strong>{{ compareAsset.name }}</strong>
					<span>{{ formatPrice(compareAsset.price) }}</span>
				</div>
				<div class="comparison-track">
					<div
						class="comparison-fill"
						:class="{
							low: (compareAsset.price || 0) === comparisonStats.min,
							high: (compareAsset.price || 0) === comparisonStats.max,
						}"
						:style="{ width: `${getPricePosition(compareAsset.price)}%` }"
					></div>
				</div>
			</div>
		</div>

		<div class="stats-grid">
			<div class="stat-box">
				<span>Lowest</span>
				<strong>{{ formatPrice(comparisonStats.min) }}</strong>
			</div>
			<div class="stat-box">
				<span>Average</span>
				<strong>{{ formatPrice(comparisonStats.average) }}</strong>
			</div>
			<div class="stat-box">
				<span>Highest</span>
				<strong>{{ formatPrice(comparisonStats.max) }}</strong>
			</div>
		</div>
	</div>

	<div v-else-if="asset" class="selected-asset">
		<div class="panel-header">
			<div>
				<h3>{{ asset.name }}</h3>
				<p>Selected asset details and market position.</p>
			</div>
			<button @click="emit('close')" class="close-btn">✕</button>
		</div>

		<div class="asset-detail-body">
			<div class="detail-row">
				<span class="label">Description</span>
				<span class="value">{{ asset.description }}</span>
			</div>
			<div class="detail-row">
				<span class="label">Price</span>
				<span class="value strong">{{ formatPrice(asset.price) }}</span>
			</div>
			<div class="detail-row">
				<span class="label">ID</span>
				<span class="value mono">{{ asset.id }}</span>
			</div>
			<div class="detail-row">
				<span class="label">Created</span>
				<span class="value">{{ formatDate(asset.createdAt) }}</span>
			</div>
		</div>

		<div class="distribution-card">
			<div class="distribution-copy">
				<strong>Price Distribution</strong>
				<span>{{ getPricePercentile(asset.price) }}th percentile</span>
			</div>
			<div class="distribution-track">
				<div class="distribution-fill" :style="{ width: `${getPricePosition(asset.price)}%` }"></div>
				<div class="distribution-marker" :style="{ left: `${getPricePosition(asset.price)}%` }"></div>
			</div>
			<div class="distribution-scale">
				<span>{{ formatPrice(stats.min) }}</span>
				<span>{{ formatPrice(stats.median) }}</span>
				<span>{{ formatPrice(stats.max) }}</span>
			</div>
		</div>
	</div>

	<div v-else class="empty-detail">
		<p>Select an asset to view details, or compare two assets to open the chart.</p>
	</div>
</template>

<style scoped>
.selected-asset,
.comparison-chart-container,
.empty-detail {
	background: linear-gradient(135deg, rgba(66, 184, 131, 0.14) 0%, rgba(51, 160, 111, 0.06) 100%);
	border: 1px solid rgba(66, 184, 131, 0.22);
	border-radius: 16px;
	padding: 1.25rem;
}

.empty-detail {
	min-height: 220px;
	display: flex;
	align-items: center;
	justify-content: center;
	color: #6b7280;
	text-align: center;
}

.empty-detail p {
	margin: 0;
	max-width: 22rem;
}

.panel-header {
	display: flex;
	justify-content: space-between;
	align-items: flex-start;
	gap: 1rem;
	margin-bottom: 1rem;
}

.panel-header h3 {
	margin: 0;
	color: #166534;
	font-size: 1.2rem;
}

.panel-header p {
	margin: 0.35rem 0 0;
	color: #4b5563;
	font-size: 0.9rem;
}

.close-btn,
.clear-btn {
	border: none;
	border-radius: 10px;
	padding: 0.55rem 0.8rem;
	cursor: pointer;
	font-weight: 600;
}

.close-btn {
	background: white;
	color: #374151;
}

.clear-btn {
	background: #fee2e2;
	color: #b91c1c;
}

.asset-detail-body {
	display: flex;
	flex-direction: column;
	gap: 0.75rem;
}

.detail-row {
	display: flex;
	justify-content: space-between;
	gap: 1rem;
	padding-bottom: 0.6rem;
	border-bottom: 1px solid rgba(255, 255, 255, 0.6);
}

.label {
	color: #374151;
	font-weight: 600;
}

.value {
	color: #4b5563;
	text-align: right;
}

.value.strong {
	color: #166534;
	font-weight: 700;
}

.mono {
	font-family: 'Courier New', monospace;
	font-size: 0.82rem;
}

.distribution-card {
	margin-top: 1rem;
	padding: 1rem;
	background: rgba(255, 255, 255, 0.78);
	border-radius: 14px;
}

.distribution-copy {
	display: flex;
	justify-content: space-between;
	gap: 1rem;
	margin-bottom: 0.8rem;
	color: #1f2937;
}

.distribution-track,
.comparison-track {
	position: relative;
	height: 14px;
	border-radius: 999px;
	background: rgba(203, 213, 225, 0.8);
	overflow: visible;
}

.distribution-fill,
.comparison-fill {
	height: 100%;
	border-radius: 999px;
	background: linear-gradient(90deg, #42b883 0%, #0f766e 100%);
}

.distribution-marker {
	position: absolute;
	top: -4px;
	width: 10px;
	height: 22px;
	border-radius: 999px;
	background: #0f172a;
	transform: translateX(-50%);
}

.distribution-scale {
	margin-top: 0.6rem;
	display: flex;
	justify-content: space-between;
	color: #6b7280;
	font-size: 0.8rem;
}

.comparison-bars {
	display: flex;
	flex-direction: column;
	gap: 1rem;
}

.comparison-row {
	display: flex;
	flex-direction: column;
	gap: 0.45rem;
}

.comparison-meta {
	display: flex;
	justify-content: space-between;
	gap: 1rem;
	color: #1f2937;
}

.comparison-fill.low {
	background: linear-gradient(90deg, #22c55e 0%, #16a34a 100%);
}

.comparison-fill.high {
	background: linear-gradient(90deg, #f97316 0%, #ef4444 100%);
}

.stats-grid {
	margin-top: 1rem;
	display: grid;
	grid-template-columns: repeat(3, 1fr);
	gap: 0.75rem;
}

.stat-box {
	padding: 0.85rem;
	border-radius: 12px;
	background: rgba(255, 255, 255, 0.82);
	display: flex;
	flex-direction: column;
	gap: 0.35rem;
}

.stat-box span {
	color: #6b7280;
	font-size: 0.8rem;
	text-transform: uppercase;
	letter-spacing: 0.04em;
}

.stat-box strong {
	color: #166534;
	font-size: 1rem;
}

@media (max-width: 768px) {
	.panel-header,
	.distribution-copy,
	.comparison-meta,
	.detail-row {
		flex-direction: column;
		align-items: stretch;
	}

	.stats-grid {
		grid-template-columns: 1fr;
	}
}
</style>
