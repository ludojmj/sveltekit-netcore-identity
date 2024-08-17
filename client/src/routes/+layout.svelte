<script>
  // +layout.svelte
  import AuthWrapper from '$lib/oidc/AuthWrapper.svelte';
  import Header from '$lib/common/Header.svelte';
  import './variables.scss';
</script>

<svelte:head>
  <title>sveltekit-netcore-identity</title>
  <meta name="description" content="sveltekit-netcore-identity" />
  <meta name="keywords" content="SvelteKit .Net8 Identity" />
</svelte:head>

<div class="container">
  <AuthWrapper>
    <Header />
    <slot />
  </AuthWrapper>
</div>
