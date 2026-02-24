<script setup>
import { ref, onMounted } from 'vue'

const forecasts = ref([])

async function loadData() {
  try {
    const response = await fetch('/weatherforecast')
    const data = await response.json()
    console.log('weatherforecast response status:', response.status)
    console.log('weatherforecast data:', data)
    forecasts.value = data
  } catch (e) {
    console.error(e)
  }
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div>
    <h1>Weather Forecast</h1>

    <p v-if="forecasts.length === 0">데이터를 불러오는 중...</p>

    <table v-else>
      <thead>
        <tr>
          <th>날짜</th>
          <th>기온 (°C)</th>
          <th>기온 (°F)</th>
          <th>날씨</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="f in forecasts" :key="f.date">
          <td>{{ f.date }}</td>
          <td>{{ f.temperatureC }}</td>
          <td>{{ f.temperatureF }}</td>
          <td>{{ f.summary }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  border: 1px solid #ddd;
  padding: 10px 14px;
  text-align: center;
}

th {
  background-color: #4a90d9;
  color: white;
}

tr:nth-child(even) {
  background-color: #f5f5f5;
}
</style>
