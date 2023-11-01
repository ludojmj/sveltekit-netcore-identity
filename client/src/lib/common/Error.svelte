<script>
  // Error.svelte
  import { tokens } from '$lib/store.js';
  import { apiErrMsg } from '$lib/const.js';
  export let msgErr, hasReset;

  $: if (msgErr === apiErrMsg.unauthorized) $tokens = null;
</script>

{#if msgErr}
  <div class="alert alert-danger alert-dismissible fade show" role="alert">
    {msgErr}
    {#if hasReset}
      <a href="/" class="btn btn-warning">Reset</a>
    {:else}
      <button class="btn btn-warning" type="button" on:click>Close</button>
    {/if}
  </div>
{/if}
