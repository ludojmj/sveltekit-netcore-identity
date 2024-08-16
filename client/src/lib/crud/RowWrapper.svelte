<script>
  // RowWrapper.svelte
  export let idx;
</script>

<div class="columns is-tablet {idx % 2 ? 'has-background-white-ter' : 'has-background-grey-lighter'}">
  <slot />
</div>
