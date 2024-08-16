<script>
  // Update.svelte
  import { crud } from '$lib/const.js';
  import Form from '$lib/crud/Form.svelte';

  const crudTitle = crud.UPDATE;
  const disabled = false;
</script>

<svelte:component this={Form} {crudTitle} {disabled} />
