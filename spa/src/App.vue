<script setup lang="ts">
import { ref, onMounted } from 'vue'

const health = ref('Loading...')

onMounted(async () => {
  try {
    const res = await fetch('/api/health')
    if (!res.ok) throw new Error(`HTTP ${res.status}`)
    health.value = await res.text()
  } catch (err) {
    health.value = `Error: ${err}`
  }
})
</script>

<template>
  <div>
    <h1>Backend Health Check</h1>
    <p>{{ health }}</p>
  </div>
</template>

<style scoped>
h1 {
  font-family: sans-serif;
  margin-bottom: 0.5rem;
}
</style>
