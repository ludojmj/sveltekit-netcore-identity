<script>
  // List.svelte
  import { crud } from '$lib/const.js';
  import { tokens } from '$lib/store.js';
  export let stuff;
</script>

<table class="table is-striped is-hoverable" summary="List of stuff">
  <caption>.List of stuff</caption>
  <thead>
    <tr>
      <th scope="col">Label</th>
      <th scope="col">Description</th>
      <th scope="col">CreatedAt</th>
      <th scope="col">UpdatedAt</th>
      <th colspan="3" />
    </tr>
  </thead>
  <tbody>
    {#each stuff.datumList || [] as stuffDatum}
      <tr
        id={stuffDatum.id}
        class={stuffDatum.user.id === $tokens.idTokenPayload.sub ? 'table-success' : 'table-danger'}
      >
        <td data-label="Label">{stuffDatum.label}</td>
        <td data-label="Description">{stuffDatum.description}</td>
        <td data-label="createdAt">
          <code>
            {#if stuffDatum.createdAt}
              {new Date(stuffDatum.createdAt).toLocaleString()}
            {:else}
              -
            {/if}
          </code>
        </td>
        <td data-label="updatedAt">
          <code>
            {#if stuffDatum.updatedAt}
              {new Date(stuffDatum.updatedAt).toLocaleString()}
            {:else}
              -
            {/if}
          </code>
        </td>
        <td>
          <a href={`/${stuffDatum.id}/${crud.READ}`} class="button is-secondary">{crud.READ}</a>
        </td>
        {#if stuffDatum.user.id === $tokens.idTokenPayload.sub}
          <td>
            <a href={`/${stuffDatum.id}/${crud.UPDATE}`} class="button is-warning">{crud.UPDATE}</a>
          </td>
          <td>
            <a href={`/${stuffDatum.id}/${crud.DELETE}`} class="button is-danger">{crud.DELETE}</a>
          </td>
        {:else}
          <td colspan="2">
            Owned by:
            <code>
              {stuffDatum.user.givenName}
              {stuffDatum.user.familyName}
            </code>
          </td>
        {/if}
      </tr>
    {/each}
  </tbody>
</table>
