<script>
  // +page.svelte
  import { tokens } from '$lib/store.js';
  import CrudManager from '$lib/common/CrudManager.svelte';
</script>

<section class="section has-text-center">
  <CrudManager />
</section>
<section class="section">
  <pre>{JSON.stringify($tokens, null, '\t')}</pre>
</section>
