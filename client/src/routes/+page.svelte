<script>
  // +page.svelte
  import { tokens } from '$lib/store.js';
  import CrudManager from '$lib/crud/CrudManager.svelte';
</script>

<CrudManager />

<pre class="is-hidden">{JSON.stringify($tokens, null, '\t')}</pre>
