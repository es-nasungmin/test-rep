<script setup></script>

<template>
  <div class="container">
    <nav class="nav">
      <RouterLink to="/">홈</RouterLink>
      <RouterLink to="/todo">Todo</RouterLink>
    </nav>
    <RouterView />
  </div>
</template>

<style scoped>
.container {
  max-width: 800px;
  margin: 40px auto;
  padding: 0 20px;
  font-family: sans-serif;
}

h1 {
  margin-bottom: 20px;
}

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

.nav {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
}

.nav a {
  color: #4a90d9;
  text-decoration: none;
  font-weight: bold;
}

.nav a.router-link-active {
  border-bottom: 2px solid #4a90d9;
}
</style>
