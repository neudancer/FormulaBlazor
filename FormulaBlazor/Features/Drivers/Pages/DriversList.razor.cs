﻿using FormulaBlazor.Features.Common.Ergast;
using FormulaBlazor.Features.Common.Wikipedia;
using FormulaBlazor.Features.Drivers.DTO;
using FormulaBlazor.Features.Drivers.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FormulaBlazor.Features.Drivers.Pages;

public partial class DriversList : ComponentBase
{
    [Inject]
    private IBaseErgastClient _client { get; set; }

    [Inject]
    private NavigationManager _navigation { get; set; }

    [Inject]
    private IWikiBaseClient _wikiBaseClient { get; set; }

    private DriverStandingDto _driverStanding = new();

    private MudTable<DriverDto> mudTable;
    private int selectedRowNumber = -1;
    private DriverDto? _selectedDriver;

    private WikipediaSummary? WikipediaSummary = new();

    //private bool driverSelected = _selectedDriver == null;

    private bool _popOverOpen = false;
    private bool _hidePosition;
    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        //await InitializeDriverStandings();
        await base.OnInitializedAsync();
    }

    private async Task InitializeDriverStandings()
    {
        _loading = true;
        _selectedDriver = null;
        var data = await _client.GetAsync<Response<MRData>>("current/driverStandings.json");
        var oldStand = data.Root.DriverStandingsTable.StandingsLists[0];
        _driverStanding = oldStand.MapToDriverList();
        _loading = false;
    }

    private async Task RowClickEvent(TableRowClickEventArgs<DriverDto> driver)
    {
        if (_selectedDriver == null)
        {
            WikipediaSummary = await _wikiBaseClient.GetDriverSummary(
                driver.Item.GivenName + "_" + driver.Item.FamilyName
            );
            _selectedDriver = driver.Item;
            _popOverOpen = true;
        }
        else
        {
            WikipediaSummary = null;
            _selectedDriver = null;
            _popOverOpen = false;
        }
    }

    private string SelectedRowClassFunc(DriverDto element, int rowNumber)
    {
        if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            //clickedEvents.Add("Selected Row: None");
            //StateHasChanged();
            return string.Empty;
        }
        else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
        {
            selectedRowNumber = rowNumber;
            //StateHasChanged();
            //clickedEvents.Add($"Selected Row: {rowNumber}");
            return "selected";
        }
        else
        {
            //StateHasChanged();
            return string.Empty;
        }
    }
}