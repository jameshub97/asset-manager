<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useAssetStore } from '@/stores/assetstore'
import type { Asset } from '@/services/api'
import { Line } from 'vue-chartjs'
import {
	CategoryScale,
	Chart as ChartJS,
	type ChartData,
	type ChartOptions,
	Filler,
	Legend,
	LineElement,
	LinearScale,
	PointElement,
	type TooltipItem,
	Tooltip,
} from 'chart.js'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend, Filler)

const props = defineProps<{
	asset: Asset | null
}>()

const emit = defineEmits<{
	close: []
}>()

const store = useAssetStore()
const chartView = ref<'bars' | 'line'>('bars')

watch(
	() => props.asset,
	(currentAsset) => {
		store.syncComparisonSelection(currentAsset)
	},
	{ immediate: true },
)

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

const comparisonRange = computed(() => {
	return Math.max(comparisonStats.value.max - comparisonStats.value.min, 1)
})

const variationSeries = computed(() => {
	const average = comparisonStats.value.average
	const deltas = store.comparisonAssets.map((item) => ({
		id: item.id,
		name: item.name,
		price: item.price || 0,
		delta: (item.price || 0) - average,
	}))
	const maxDelta = Math.max(...deltas.map((item) => Math.abs(item.delta)), 1)

	return deltas.map((item) => ({
		...item,
		width: `${Math.max((Math.abs(item.delta) / maxDelta) * 100, item.delta === 0 ? 0 : 10)}%`,
		isPositive: item.delta > 0,
		isNeutral: item.delta === 0,
	}))
})

const comparisonLineData = computed<ChartData<'line'>>(() => {
	return {
		labels: store.comparisonAssets.map((item) => item.name),
		datasets: [
			{
				label: 'Price',
				data: store.comparisonAssets.map((item) => item.price || 0),
				borderColor: '#0f766e',
				backgroundColor: 'rgba(15, 118, 110, 0.18)',
				pointBackgroundColor: store.comparisonAssets.map((item) => isFreeAsset(item.price) ? '#94a3b8' : '#42b883'),
				pointBorderColor: '#ffffff',
				pointBorderWidth: 2,
				pointRadius: 5,
				tension: 0.35,
				fill: true,
			},
		],
	}
})

const comparisonLineOptions = computed<ChartOptions<'line'>>(() => {
	return {
		responsive: true,
		maintainAspectRatio: false,
		plugins: {
			legend: {
				display: false,
			},
			tooltip: {
				callbacks: {
					label: (context: TooltipItem<'line'>) => {
						const numericValue = typeof context.raw === 'number' ? context.raw : Number(context.raw ?? 0)
						return numericValue <= 0 ? 'Free' : formatPrice(numericValue)
					},
				},
			},
		},
		scales: {
			x: {
				grid: {
					display: false,
				},
				ticks: {
					color: '#475569',
					maxRotation: 0,
					autoSkip: false,
				},
			},
			y: {
				beginAtZero: true,
				grace: '8%',
				ticks: {
					color: '#475569',
					callback: (value: string | number) => {
						const numericValue = typeof value === 'number' ? value : Number(value)
						return numericValue <= 0 ? 'Free' : formatPrice(numericValue)
					},
				},
				grid: {
					color: 'rgba(148, 163, 184, 0.2)',
				},
			},
		},
	}
})

const stagedComparisonAsset = computed(() => {
	return store.comparisonAssets[0] ?? props.asset
})

const getPricePosition = (price?: number) => {
	if (!price) return 0
	return ((price - stats.value.min) / priceRange.value) * 100
}

const getComparisonWidth = (price?: number) => {
	if (!price) return 0

	if (store.comparisonAssets.length <= 1) {
		return 100
	}

	if (comparisonStats.value.max === comparisonStats.value.min) {
		return 100
	}

	const normalized = ((price - comparisonStats.value.min) / comparisonRange.value) * 100
	return Math.max(normalized, 14)
}

const getComparisonDelta = (price?: number) => {
	if (!price) return '$0.00'

	const delta = price - comparisonStats.value.min
	return delta <= 0 ? 'Baseline' : `+${formatPrice(delta)}`
}

const isFreeAsset = (price?: number) => {
	return !price || price <= 0
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

const formatDelta = (value: number) => {
	if (value === 0) return 'At average'
	return `${value > 0 ? '+' : '-'}${formatPrice(Math.abs(value))}`
}
</script>

<template>
	<div v-if="store.comparisonAssets.length >= 2" class="comparison-chart-container">
		<div class="panel-header">
			<div>
				<h3>Price Comparison</h3>
				<p>Compare selected assets against each other.</p>
			</div>
			<div class="chart-actions">
				<div class="chart-toggle" role="tablist" aria-label="Comparison chart view">
					<button
						type="button"
						class="chart-toggle-btn"
						:class="{ active: chartView === 'bars' }"
						@click="chartView = 'bars'"
					>
						Bars
					</button>
					<button
						type="button"
						class="chart-toggle-btn"
						:class="{ active: chartView === 'line' }"
						@click="chartView = 'line'"
					>
						Line
					</button>
				</div>
				<button class="clear-btn" @click="store.clearComparison()">Clear</button>
			</div>
		</div>

		<div v-if="chartView === 'bars'" class="comparison-bars">
			<div v-for="compareAsset in store.comparisonAssets" :key="compareAsset.id" class="comparison-row">
				<div class="comparison-meta">
					<div class="comparison-meta-copy">
						<strong>{{ compareAsset.name }}</strong>
						<small>{{ isFreeAsset(compareAsset.price) ? 'Free' : getComparisonDelta(compareAsset.price) }}</small>
					</div>
					<span>{{ isFreeAsset(compareAsset.price) ? 'Free' : formatPrice(compareAsset.price) }}</span>
				</div>
				<div v-if="!isFreeAsset(compareAsset.price)" class="comparison-track">
					<div
						class="comparison-fill"
						:class="{
							low: (compareAsset.price || 0) === comparisonStats.min,
							high: (compareAsset.price || 0) === comparisonStats.max,
						}"
						:style="{ width: `${getComparisonWidth(compareAsset.price)}%` }"
					></div>
				</div>
				<p v-else class="free-note">Free item, no price bar shown.</p>
			</div>
		</div>

		<div v-else class="line-chart-panel">
			<Line :data="comparisonLineData" :options="comparisonLineOptions" />
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

		<div class="variation-panel">
			<div class="variation-header">
				<strong>Price Variation</strong>
				<span>Difference from comparison average {{ formatPrice(comparisonStats.average) }}</span>
			</div>

			<div class="variation-list">
				<div v-for="item in variationSeries" :key="item.id" class="variation-row">
					<div class="variation-meta">
						<span>{{ item.name }}</span>
						<small>{{ formatDelta(item.delta) }}</small>
					</div>

					<div class="variation-track">
						<div
							v-if="!item.isNeutral"
							class="variation-fill"
							:class="{ positive: item.isPositive, negative: !item.isPositive }"
							:style="{ width: item.width }"
						></div>
						<div v-else class="variation-neutral">Average</div>
					</div>
				</div>
			</div>
		</div>
	</div>

	<div v-else-if="stagedComparisonAsset" class="selected-asset">
		<div class="panel-header">
			<div>
				<h3>{{ stagedComparisonAsset.name }}</h3>
				<p>Comparison starter selected. Choose one more asset to compare.</p>
			</div>
			<button @click="emit('close')" class="close-btn">✕</button>
		</div>

		<div class="comparison-prompt">
			<span class="comparison-prompt-badge">1 selected</span>
			<p>Select another asset with the Compare button to open the chart.</p>
		</div>

		<div class="asset-detail-body">
			<div class="detail-row">
				<span class="label">Description</span>
				<span class="value">{{ stagedComparisonAsset.description }}</span>
			</div>
			<div class="detail-row">
				<span class="label">Price</span>
				<span class="value strong">{{ formatPrice(stagedComparisonAsset.price) }}</span>
			</div>
			<div class="detail-row">
				<span class="label">ID</span>
				<span class="value mono">{{ stagedComparisonAsset.id }}</span>
			</div>
			<div class="detail-row">
				<span class="label">Created</span>
				<span class="value">{{ formatDate(stagedComparisonAsset.createdAt) }}</span>
			</div>
		</div>

		<div class="distribution-card">
			<div class="distribution-copy">
				<strong>Price Distribution</strong>
				<span>{{ getPricePercentile(stagedComparisonAsset.price) }}th percentile</span>
			</div>
			<div class="distribution-track">
				<div class="distribution-fill" :style="{ width: `${getPricePosition(stagedComparisonAsset.price)}%` }"></div>
				<div class="distribution-marker" :style="{ left: `${getPricePosition(stagedComparisonAsset.price)}%` }"></div>
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

.chart-actions {
	display: flex;
	align-items: center;
	gap: 0.75rem;
	flex-wrap: wrap;
}

.chart-toggle {
	display: inline-flex;
	padding: 0.2rem;
	border-radius: 12px;
	background: rgba(255, 255, 255, 0.9);
	border: 1px solid rgba(148, 163, 184, 0.28);
}

.chart-toggle-btn {
	border: none;
	background: transparent;
	color: #475569;
	padding: 0.45rem 0.8rem;
	border-radius: 10px;
	cursor: pointer;
	font-size: 0.85rem;
	font-weight: 700;
}

.chart-toggle-btn.active {
	background: #42b883;
	color: white;
}

.asset-detail-body {
	display: flex;
	flex-direction: column;
	gap: 0.75rem;
}

.comparison-prompt {
	display: flex;
	align-items: center;
	gap: 0.75rem;
	padding: 0.85rem 1rem;
	margin-bottom: 1rem;
	border-radius: 12px;
	background: rgba(255, 255, 255, 0.78);
	border: 1px solid rgba(66, 184, 131, 0.16);
}

.comparison-prompt p {
	margin: 0;
	color: #4b5563;
	font-size: 0.9rem;
}

.comparison-prompt-badge {
	padding: 0.3rem 0.6rem;
	border-radius: 999px;
	background: #dcfce7;
	color: #166534;
	font-size: 0.78rem;
	font-weight: 700;
	white-space: nowrap;
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

.line-chart-panel {
	height: 260px;
	padding: 0.75rem 0.5rem 0.25rem;
	border-radius: 14px;
	background: rgba(255, 255, 255, 0.78);
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

.comparison-meta-copy {
	display: flex;
	flex-direction: column;
	gap: 0.2rem;
}

.comparison-meta-copy small {
	color: #6b7280;
	font-size: 0.78rem;
	font-weight: 600;
}

.free-note {
	margin: 0;
	color: #166534;
	font-size: 0.85rem;
	font-weight: 600;
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

.variation-panel {
	margin-top: 1rem;
	padding: 1rem;
	border-radius: 14px;
	background: rgba(255, 255, 255, 0.82);
}

.variation-header {
	display: flex;
	justify-content: space-between;
	gap: 1rem;
	margin-bottom: 0.85rem;
	color: #1f2937;
}

.variation-header strong {
	font-size: 0.98rem;
}

.variation-header span {
	font-size: 0.82rem;
	color: #6b7280;
	text-align: right;
}

.variation-list {
	display: flex;
	flex-direction: column;
	gap: 0.8rem;
}

.variation-row {
	display: grid;
	grid-template-columns: minmax(140px, 180px) 1fr;
	gap: 0.9rem;
	align-items: center;
}

.variation-meta {
	display: flex;
	flex-direction: column;
	gap: 0.2rem;
	color: #1f2937;
}

.variation-meta small {
	color: #6b7280;
	font-size: 0.78rem;
	font-weight: 600;
}

.variation-track {
	min-height: 12px;
	display: flex;
	align-items: center;
	background: rgba(226, 232, 240, 0.75);
	border-radius: 999px;
	overflow: hidden;
}

.variation-fill {
	height: 12px;
	border-radius: 999px;
}

.variation-fill.positive {
	background: linear-gradient(90deg, #f59e0b 0%, #ef4444 100%);
}

.variation-fill.negative {
	background: linear-gradient(90deg, #10b981 0%, #14b8a6 100%);
}

.variation-neutral {
	padding-left: 0.6rem;
	font-size: 0.76rem;
	font-weight: 700;
	color: #475569;
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
	.comparison-prompt,
	.distribution-copy,
	.comparison-meta,
	.variation-header,
	.detail-row {
		flex-direction: column;
		align-items: stretch;
	}

	.chart-actions {
		align-items: stretch;
	}

	.stats-grid {
		grid-template-columns: 1fr;
	}

	.variation-row {
		grid-template-columns: 1fr;
	}
}
</style>
