<script>
  // +layout.svelte
  import AuthWrapper from '$lib/oidc/AuthWrapper.svelte';
  import './variables.scss';
</script>

<svelte:head>
  <title>sveltekit-netcore-identity</title>
  <meta name="description" content="sveltekit-netcore-identity" />
  <meta name="keywords" content="SvelteKit .Net6 Identity" />
</svelte:head>

<AuthWrapper>
  <slot />
</AuthWrapper>
