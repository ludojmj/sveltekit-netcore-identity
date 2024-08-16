<script>
  // Row.svelte
  import RowWrapper from '$lib/crud/RowWrapper.svelte';
  import CrudButtons from '$lib/crud/CrudButtons.svelte';

  export let stuffDatum, idx;
</script>

<RowWrapper {idx}>
  <div class="column column is-3">{stuffDatum.label}</div>

  <div class="column column is-3">{stuffDatum.description}</div>

  <div class="column is-2">
    <small>
      {#if stuffDatum.createdAt}
        {new Date(stuffDatum.createdAt).toLocaleString()}
      {:else}
        -
      {/if}
    </small>
  </div>

  <div class="column is-2">
    <small>
      {#if stuffDatum.updatedAt}
        {new Date(stuffDatum.updatedAt).toLocaleString()}
      {:else}
        -
      {/if}
    </small>
  </div>

  <div class="column is-2">
    <CrudButtons {stuffDatum} />
  </div>
</RowWrapper>
