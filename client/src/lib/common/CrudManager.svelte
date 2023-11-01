<script>
  // CrudManager.svelte
  import { onMount } from 'svelte';
  import { crud } from '$lib/const.js';
  import { isLoading } from '$lib/store.js';
  import { apiGetStuffListAsync, apiSearchStuffAsync, apiGotoPageAsync } from '$lib/api.js';
  import List from '$lib/common/List.svelte';
  import Error from '$lib/common/Error.svelte';
  import Loading from '$lib/common/Loading.svelte';
  import Logo from '$lib/common/Logo.svelte';
  import Logout from '$lib/oidc/Logout.svelte';

  let searchTerm = '';
  let stuff = {};
  onMount(async () => {
    stuff = await apiGetStuffListAsync();
  });

  const init = (el) => {
    el.focus();
  };

  const handleReset = async () => {
    searchTerm = '';
    stuff = await apiGetStuffListAsync();
  };

  const handleSearch = async () => {
    if (searchTerm) {
      stuff = await apiSearchStuffAsync(searchTerm);
      return;
    }

    handleReset();
  };

  const handlePage = async (event) => {
    searchTerm = '';
    const page = event.currentTarget.value === '+' ? stuff.page + 1 : stuff.page - 1;
    stuff = await apiGotoPageAsync(page);
  };
</script>

{#if $isLoading}
  <Loading />
{:else if stuff.error}
  <Error msgErr={stuff.error} hasReset={false} />
{:else}
  <form on:submit|preventDefault={handleSearch}>
    <div class="row">
      <div class="col">
        <button type="reset" class="btn" on:click={handleReset}>
          <Logo />
        </button>
      </div>
      <div class="col">
        <div class="input-group">
          <input
            bind:value={searchTerm}
            use:init
            class="form-control"
            type="search"
            placeholder="Filter"
            aria-label="Filter"
            maxLength="20"
          />
          <button class="btn btn-outline-secondary" type="submit">Search</button>
        </div>
      </div>
      <div class="col">
        <ul class="pagination">
          <li class="page-item">
            <button
              class="btn btn-primary"
              value="-"
              on:click|preventDefault={handlePage}
              disabled={!stuff.page || stuff.page === 1}
            >
              &laquo;
            </button>
          </li>
          <li class="page-item">
            <div class="form-control">
              Page {stuff.page ? stuff.page : 0}/{stuff.totalPages ? stuff.totalPages : 0}
            </div>
          </li>
          <li class="page-item">
            <button
              class="btn btn-primary"
              value="+"
              on:click|preventDefault={handlePage}
              disabled={stuff.page === stuff.totalPages}
            >
              &raquo;
            </button>
          </li>
        </ul>
      </div>
      <div class="col">
        <a href={`/${crud.CREATE}`} class="btn btn-success">
          {crud.CREATE}
        </a>
      </div>
      <div class="col">
        <Logout />
      </div>
    </div>
  </form>
  <List {stuff} />
{/if}
