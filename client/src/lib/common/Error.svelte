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
    {#if hasReset}
      <a data-sveltekit-reload href="/" class="delete"> </a>
    {:else}
      <button class="delete" on:click />
    {/if}
  </div>
{/if}
