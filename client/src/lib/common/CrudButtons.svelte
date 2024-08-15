<script>
  // CrudButtons.svelte
  import { crud } from '$lib/const.js';
  import { tokens } from '$lib/store.js';

  export let stuffDatum;
</script>

<a href={`/${stuffDatum.id}/${crud.READ}`} class="button is-small is-info">{crud.READ}</a>
{#if stuffDatum.user.id === $tokens.idTokenPayload.sub}
  <a href={`/${stuffDatum.id}/${crud.UPDATE}`} class="button is-small is-warning">{crud.UPDATE}</a>
  <a href={`/${stuffDatum.id}/${crud.DELETE}`} class="button is-small is-danger">{crud.DELETE}</a>
{:else}
  Owned by:
  <code>
    {stuffDatum.user.givenName}
    {stuffDatum.user.familyName}
  </code>
{/if}
