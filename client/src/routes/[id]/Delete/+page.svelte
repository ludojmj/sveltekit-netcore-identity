<script>
  // Delete.svelte
  import { crud } from '$lib/const.js';
  import Form from '$lib/common/Form.svelte';

  const crudTitle = crud.DELETE;
  const disabled = true;
</script>

<svelte:component this={Form} {crudTitle} {disabled} />
