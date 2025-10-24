<template>
  <div class="landing-page">
    <div class="hero">
      <h1>Welcome to Optiviera</h1>
      <p>Modern Ticket Management SaaS Platform</p>
      <div class="cta-buttons">
        <NuxtLink to="/login">
          <Button label="Login" icon="pi pi-sign-in" />
        </NuxtLink>
        <NuxtLink to="/register">
          <Button label="Get Started" icon="pi pi-user-plus" severity="success" />
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})
</script>

<style scoped>
.landing-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.hero {
  text-align: center;
  color: white;
  padding: 2rem;
}

.hero h1 {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.hero p {
  font-size: 1.5rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.cta-buttons {
  display: flex;
  gap: 1rem;
  justify-content: center;
}
</style>
