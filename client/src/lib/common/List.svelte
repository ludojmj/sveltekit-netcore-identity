<script>
  // List.svelte
  import Row from '$lib/common/Row.svelte';
  export let stuff;
</script>

<div class="columns has-background-info has-text-white has-text-weight-bold">
  <div class="column">Label</div>
  <div class="column">Description</div>
  <div class="column is-2">CreatedAt</div>
  <div class="column is-2">UpdatedAt</div>
  <div class="column"></div>
</div>

{#each stuff.datumList || [] as stuffDatum, idx}
  <Row {stuffDatum} {idx} />
{/each}
