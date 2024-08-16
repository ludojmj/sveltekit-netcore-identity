<script>
  // +page.svelte
  import { tokens } from '$lib/store.js';
  import CrudManager from '$lib/crud/CrudManager.svelte';
</script>

<section class="section has-text-center">
  <CrudManager />
</section>
{#if false}
  <section class="section">
    <pre>{JSON.stringify($tokens, null, '\t')}</pre>
  </section>
{/if}
