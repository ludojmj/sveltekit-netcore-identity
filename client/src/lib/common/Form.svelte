<script>
  // CommonForm.svelte
  import { onMount } from 'svelte';
  import { page } from '$app/stores';
  import { isLoading } from '$lib/store.js';
  import { crud } from '../const.js';
  import { crudApiCallAsync, getItemAsync } from '$lib/api.js';
  import { handleEscape, handleFormError, setFocus } from '$lib/tools.js';
  import Error from '$lib/common/Error.svelte';
  import Loading from '$lib/common/Loading.svelte';
  export let crudTitle, disabled;

  const id = $page.params.id;

  let stuffDatum = {};
  let initialDatum = {};
  onMount(async () => {
    const itemObj = await getItemAsync(crudTitle, id);
    stuffDatum = itemObj.item;
    initialDatum = itemObj.initialItem;
  });

  let error = '';
  const handleSubmitAsync = async () => {
    error = handleFormError(crudTitle, stuffDatum, initialDatum);
    if (error) {
      return;
    }

    stuffDatum = await crudApiCallAsync(crudTitle, stuffDatum);
  };
</script>

{#if stuffDatum?.error}
  <Error msgErr={stuffDatum.error} hasReset={true} />
{:else if $isLoading || !stuffDatum}
  <Loading />
{:else}
  <form class="alert alert-secondary" on:submit|preventDefault={handleSubmitAsync}>
    <header
      class="modal-header alert
        {(crudTitle === crud.CREATE && 'alert-success') ||
        (crudTitle === crud.READ && 'alert-dark') ||
        (crudTitle === crud.UPDATE && 'alert-warning') ||
        (crudTitle === crud.DELETE && 'alert-danger')}"
    >
      <h5 class="modal-title">
        {crudTitle}
      </h5>
      <a href="/" class="btn-close">.</a>
    </header>

    <label class="form-label" for="user">Owner:</label>
    <input
      on:keyup|preventDefault={handleEscape}
      class="form-control fw-bold"
      type="text"
      id="user"
      name="user"
      value={stuffDatum.user ? stuffDatum.user.givenName : 'Current user'}
      aria-label="User"
      disabled
    />

    <label class="form-label" for="label">Label:</label>
    <input
      on:keyup|preventDefault={handleEscape}
      bind:value={stuffDatum.label}
      use:setFocus
      minlength="3"
      maxlength="80"
      required
      class="form-control"
      type="text"
      maxLength="79"
      placeholder="Label"
      {disabled}
    />

    <label class="form-label" for="description">Description:</label>
    <input
      on:keyup|preventDefault={handleEscape}
      bind:value={stuffDatum.description}
      class="form-control"
      type="text"
      maxLength="79"
      placeholder="Description"
      {disabled}
    />

    <label class="form-label" for="otherInfo">Other info:</label>
    <textarea
      on:keyup|preventDefault={handleEscape}
      bind:value={stuffDatum.otherInfo}
      class="form-control"
      rows="5"
      maxLength="399"
      placeholder="Other info"
      {disabled}
    />

    <footer>
      <a href="/" class="btn btn-danger">Cancel</a>
      <button class="btn btn-success" type="submit">Confirm</button>
    </footer>

    <Error msgErr={error} hasReset={false} on:click={() => (error = '')} />
  </form>
{/if}
