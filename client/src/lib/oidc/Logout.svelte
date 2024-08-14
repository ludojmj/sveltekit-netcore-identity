<script>
  // Logout.svelte
  import { tokens } from '$lib/store.js';
  import { logoutAsync } from '$lib/oidc.js';
</script>

{#if $tokens}
  <button class="button is-danger" on:click={logoutAsync}>
    Logout &nbsp;

    <span class="tag is-rounded">
      {$tokens.accessTokenPayload.name}
    </span>
  </button>
{/if}
