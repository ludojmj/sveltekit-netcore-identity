<script>
  // CrudManager.svelte
  import { onMount } from 'svelte';
  import { crud } from '$lib/const.js';
  import { isLoading } from '$lib/store.js';
  import { apiGetStuffListAsync, apiSearchStuffAsync, apiGotoPageAsync } from '$lib/api.js';
  import List from '$lib/crud/List.svelte';
  import Error from '$lib/common/Error.svelte';
  import Loading from '$lib/common/Loading.svelte';

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

  const handleEscapeOrEnter = (event) => {
    if (event.code == 'Escape') {
      handleReset();
    }

    if (event.code == 'Enter' || event.code == 'NumpadEnter') {
      handleSearch();
    }
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

{#if stuff.error}
  <Error msgErr={stuff.error} hasReset={false} />
{:else if $isLoading || !stuff.datumList}
  <Loading />
{:else}
  <div class="box has-background-primary">
    <form on:submit|preventDefault={handleSearch}>
      <div class="columns">
        <div class="column is-4">
          <div class="pagination">
            <ul class="pagination-list">
              <li>
                <button
                  class="button is-primary-light"
                  value="-"
                  on:click|preventDefault={handlePage}
                  disabled={!stuff.page || stuff.page === 1}
                >
                  &laquo;
                </button>
              </li>
              <li>
                <div class="field">
                  Page {stuff.page ? stuff.page : 0}/{stuff.totalPages ? stuff.totalPages : 0}
                </div>
              </li>
              <li>
                <button
                  class="button is-primary-light"
                  value="+"
                  on:click|preventDefault={handlePage}
                  disabled={stuff.page === stuff.totalPages}
                >
                  &raquo;
                </button>
              </li>
            </ul>
          </div>
        </div>

        <div class="column is-6">
          <div class="field has-addons">
            <div class="control has-icons-right">
              <input
                bind:value={searchTerm}
                use:init
                on:keyup|preventDefault={handleEscapeOrEnter}
                class="input"
                type="search"
                placeholder="Filter"
                aria-label="Filter"
                maxLength="20"
              />
              <span class="icon is-right">
                <i
                  on:click={handleReset}
                  on:keydown={handleReset}
                  class="delete"
                  tabindex="0"
                  role="button"
                />
              </span>
            </div>
            <div class="control">
              <button class="button is-primary" type="submit">Search</button>
            </div>
          </div>
        </div>

        <div class="column is-2">
          <a href={`/${crud.CREATE}`} class="button is-success">
            {crud.CREATE}
          </a>
        </div>
      </div>
    </form>

    <List {stuff} />
  </div>
{/if}
