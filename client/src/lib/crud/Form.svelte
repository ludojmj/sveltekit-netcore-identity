<script>
  // CommonForm.svelte
  import { onMount } from 'svelte';
  import { page } from '$app/stores';
  import { isLoading } from '$lib/store.js';
  import { crud } from '$lib/const.js';
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
  <section
    class="panel {(crudTitle === crud.CREATE && 'is-success') ||
      (crudTitle === crud.READ && 'is-info') ||
      (crudTitle === crud.UPDATE && 'is-warning') ||
      (crudTitle === crud.DELETE && 'is-danger')}"
  >
    <p class="panel-heading">{crudTitle}</p>
    <form class="box" on:submit|preventDefault={handleSubmitAsync}>
      <label class="label" for="user">Owner:</label>
      <input
        on:keyup|preventDefault={handleEscape}
        class="input has-text-weight-bold"
        type="text"
        id="user"
        name="user"
        value={stuffDatum.user ? stuffDatum.user.givenName : 'Current user'}
        aria-label="User"
        disabled
      />

      <label class="label" for="label">Label:</label>
      <input
        on:keyup|preventDefault={handleEscape}
        bind:value={stuffDatum.label}
        use:setFocus
        minlength="3"
        maxlength="80"
        required
        class="input"
        type="text"
        maxLength="79"
        placeholder="Label"
        {disabled}
      />

      <label class="label" for="description">Description:</label>
      <input
        on:keyup|preventDefault={handleEscape}
        bind:value={stuffDatum.description}
        class="input"
        type="text"
        maxLength="79"
        placeholder="Description"
        {disabled}
      />

      <label class="label" for="otherInfo">Other info:</label>
      <textarea
        on:keyup|preventDefault={handleEscape}
        bind:value={stuffDatum.otherInfo}
        class="textarea"
        rows="5"
        maxLength="399"
        placeholder="Other info"
        {disabled}
      ></textarea>

      <footer>
        <a href="/" class="button is-danger">Cancel</a>
        <button class="button is-success" type="submit">Confirm</button>
      </footer>

      <Error msgErr={error} hasReset={false} on:click={() => (error = '')} />
    </form>
  </section>
{/if}
