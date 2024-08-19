<script>
  // Row.svelte
  import CrudButtons from '$lib/crud/CrudButtons.svelte';

  export let stuffDatum, idx;
</script>

<div class="columns is-multiline is-tablet {idx % 2 ? 'has-background-white-ter' : 'has-background-grey-lighter'}">
  <div class="column is-3 enable-line-break">{stuffDatum.label}</div>

  <div class="column is-3 enable-line-break">{stuffDatum.description}</div>

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
</div>
