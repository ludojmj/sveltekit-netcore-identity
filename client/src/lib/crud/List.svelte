<script>
  // List.svelte
  import Row from '$lib/crud/Row.svelte';

  export let stuff;
</script>

<div class="columns is-hidden-mobile has-background-info has-text-white has-text-weight-bold">
  <div class="column is-3">Label</div>
  <div class="column is-3">Description</div>
  <div class="column is-2">CreatedAt</div>
  <div class="column is-2">UpdatedAt</div>
  <div class="column is-2"></div>
</div>

{#each stuff.datumList || [] as stuffDatum, idx}
  <Row {stuffDatum} {idx} />
{/each}
