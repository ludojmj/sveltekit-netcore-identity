<script>
  // Login.svelte
  import { tokens } from '$lib/store.js';
  import { loginAsync } from '$lib/oidc.js';
  import Logo from '$lib/common/Logo.svelte';
</script>

{#if !$tokens}
  <main class="container text-center">
    <div class="row">
      <div class="col">
        <Logo />
      </div>
      <div class="col">
        <h1 class="title">sveltekit-netcore-identity</h1>
      </div>
      <div class="col">
        <button class="btn btn-primary" on:click={async () => await loginAsync()}> Login </button>
      </div>
    </div>
  </main>
{/if}
