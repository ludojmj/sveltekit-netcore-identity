<script>
  // RowWrapper.svelte
  import { tokens } from '$lib/store.js';
  export let stuffDatum;
</script>

<div
  class="columns is-tablet {stuffDatum.user.id === $tokens.idTokenPayload.sub ? 'has-background-success' : 'has-background-danger'}"
  id={stuffDatum.id}
>
  <slot />
</div>
