<script>
  // Error.svelte
  import { tokens } from '$lib/store.js';
  import { apiErrMsg } from '$lib/const.js';
  export let msgErr, hasReset;

  $: if (msgErr === apiErrMsg.unauthorized) $tokens = null;
</script>

{#if msgErr}
  <div class="notification is-danger is-light">
    {msgErr}
    <button class="delete" on:click />
    {#if hasReset}
      <a href="/" class="button is-warning">Reset</a>
      {:else}
      <button class="button is-warning" type="button" on:click>Close</button>
    {/if}
  </div>
{/if}
