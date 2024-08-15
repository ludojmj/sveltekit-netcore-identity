<script>
  // Row.svelte
  import RowWrapper from '$lib/common/RowWrapper.svelte';
  import CrudButtons from '$lib/common/CrudButtons.svelte';
  export let stuffDatum;
</script>

<RowWrapper {stuffDatum}>
  <div class="column has-text-weight-bold">{stuffDatum.label}</div>

  <div class="column">{stuffDatum.description}</div>

  <div class="column is-2">
    <code>
      {#if stuffDatum.createdAt}
        {new Date(stuffDatum.createdAt).toLocaleString()}
      {:else}
        -
      {/if}
    </code>
  </div>

  <div class="column is-2">
    <code>
      {#if stuffDatum.updatedAt}
        {new Date(stuffDatum.updatedAt).toLocaleString()}
      {:else}
        -
      {/if}
    </code>
  </div>

  <div class="column">
    <CrudButtons {stuffDatum} />
  </div>
</RowWrapper>
