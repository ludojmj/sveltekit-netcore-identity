<script>
  // FormWrapper.svelte
  import { crud } from '$lib/const.js';

  export let crudCmd;

  let localStyle;
  switch (crudCmd) {
    case crud.READ:
      localStyle = 'is-info';
      break;
    case crud.UPDATE:
      localStyle = 'is-warning';
      break;
    case crud.DELETE:
      localStyle = 'is-danger';
      break;
    default:
      localStyle = 'is-success';
  }
</script>

<section class="panel {localStyle}">
  <p class="panel-heading">
    {crudCmd}
  </p>
  <slot />
</section>
