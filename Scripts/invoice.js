var Partyresult = '';
var _total = 0;
var InvoiceData = '';

$(document).ready(function () {
    //debugger

    $.ajax({
        url: '/Home/GetDropdownData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var drpDwndata = data.DropDown;
            Partyresult = JSON.parse(data.PartyData);
            var dropdown = $('#drpPrtyNme');
            dropdown.empty();
            $.each(drpDwndata, function () {
                dropdown.append($('<option />').val(this.Value).text(this.Text));
            });
        },
        error: function () {
            console.log('Error fetching data from the controller.');
        }
    });

    $.ajax({
        url: '/Home/GetInvoiceData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            InvoiceData = JSON.parse(data);
            bindDataToGrid(JSON.parse(data));
        },
        error: function () {
            console.log('Error fetching data from the controller.');
        }
    });

})

$('#drpPrtyNme').on('change', function () {
    var selectedValue = $(this).val();
    var SelectedParty = $.grep(Partyresult, function (item) {
        return item.PartyMasterId == selectedValue;
    });
    $('#lblGst').text(SelectedParty[0].GST);
    $('#lblContct').text(SelectedParty[0].Contact);
});

document.getElementById("addRowButton").addEventListener("click", function () {
    
    addRowToGrid();
});


document.getElementById("calculate").addEventListener("click", function () {
    var GrandTotal = 0;
    var Gst = 0;
    var Discount = $('#txtBxDscnt').val()

    $.each(InvoiceData, function (ind, item) {
        GrandTotal += Number(item.Total);
        Gst += (Number(item.Rate) * Number(item.Qty)) / Number(item.GST);
    })
    var NetAmount = GrandTotal + Discount + Gst;
    $('#lblGrndAmnt').text(GrandTotal);
    $('#lblGstAmnt').text(Gst);
    $('#lblNetAmnt').text(NetAmount);

});

function addRowToGrid() {

    var newRow = '<tr>' +
        '<td><input type="text" class="form-control"></td>' +
        '<td><input type="text" class="form-control" ></td>' +
        '<td><input type="text" class="form-control" ></td>' +
        '<td><input type="text" class="form-control" ></td>' +
        '<td><input type="text" class="form-control" ></td>' +
        '<td><input type="text" class="form-control" ></td>' +
        '<td><input type="text" class="form-control" ></td>' +
        '<td><button onclick="saveRow(this)">Save</button></td>' +
        '</tr>';

    $('#Invoicegrid tbody').append(newRow);

    
}

function saveRow(button) {
    var row = button.parentNode.parentNode;
    var Sr = row.cells[0].querySelector("input").value;
    var ItemCode = row.cells[1].querySelector("input").value;
    var ItemName = row.cells[2].querySelector("input").value;
    var Rate = row.cells[3].querySelector("input").value;
    var QTY = row.cells[4].querySelector("input").value;
    var GST = row.cells[5].querySelector("input").value;
    var Total_ = row.cells[6].querySelector("input").value = (Rate * QTY);


    var InvoiceName = $('#txtBxInvoiceNo').val();
    var InvoiceDate = $('#dtPckrInvoice').val();
    var PartyName = $('#drpPrtyNme').find('option:selected').text();
    var PartyID = $('#drpPrtyNme').val();
    var GSTNo = $('#lblGst').text();
    var Contact = $('#lblContct').text();

    var rowData = {
        Invoicename: InvoiceName,
        Invoicedate: InvoiceDate,
        Partyname: PartyName,
        Gstno: GSTNo,
        Contact: Contact,
        Sr: Sr,
        Itemcode: ItemCode,
        Itemname: ItemName,
        Qty: QTY,
        Rate: Rate,
        Gst: GST,
        Total: Total_,
        PartyMasterId: PartyID,
        
    };
    var jsonData = JSON.stringify(rowData);

    $.ajax({
        url: '/Home/SaveRow',
        type: 'POST',
        data: jsonData,
        contentType: 'application/json; charset=utf-8',
        success: function () {
            alert("Row saved successfully!");
        },
        error: function () {
            alert("An error occurred while saving the row.");
        }
    });

}


function bindDataToGrid(data) {
    var grid = $('#Invoicegrid');
    
    var sr = 1;
    grid.empty();
    var head = $('<thead>');
    grid.append(head);
    var headers = '<tr>';
    headers += '<th>Sr No.</th>';
    
    $.each(data[0], function (key, value) {
        headers += '<th>' + key + '</th>';
    });
    headers += '<th>Action</th>';
    headers += '</tr>';
    head.append(headers)
    var tbodyData = $('<tbody>');
    grid.append(tbodyData);
    $.each(data, function (index, item) {
        var row = '<tr>';
        row += '<td>'+ sr +'</td>';
        $.each(item, function (key, value) {
            row += '<td>' + value + '</td>';
        });
        row += '<td><button onclick="saveRow(this)">Save</button></td>';
        row += '</tr>';
        tbodyData.append(row);
        sr++;
    });

    
}