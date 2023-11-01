<script>
  // Logout.svelte
  import { tokens } from '$lib/store.js';
  import { logoutAsync } from '$lib/oidc.js';
</script>

{#if $tokens}
  <button class="btn btn-danger" on:click={async () => await logoutAsync()}>
    <span>
      Logout &nbsp;

      <span class="badge bg-primary">
        {$tokens.accessTokenPayload.name}
      </span>
    </span>
  </button>
{/if}
