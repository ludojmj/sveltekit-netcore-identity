<script>
  // +page.svelte
  import { tokens } from '$lib/store.js';
  import CrudManager from '$lib/crud/CrudManager.svelte';
</script>

<CrudManager />

{#if false}
  <pre>{JSON.stringify($tokens, null, '\t')}</pre>
{/if}
