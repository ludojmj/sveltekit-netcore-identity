<script>
  // Login.svelte
  import { tokens } from '$lib/store.js';
  import { loginAsync } from '$lib/oidc.js';
  import Logo from '$lib/common/Logo.svelte';
</script>

{#if !$tokens}
  <div class="columns is-tablet">
    <div class="column">
      <Logo />
    </div>
    <div class="column">
      <h1 class="title">sveltekit-netcore-identity</h1>
    </div>
    <div class="column has-text-right">
      <button class="button is-info" on:click={loginAsync}> Login </button>
    </div>
  </div>
{/if}
