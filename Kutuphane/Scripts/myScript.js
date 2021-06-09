function myToastr(data) {
    if (data.success == 0) {
        toastr.success(data.message, data.title, { extendedTimeOut: true, timeOut: 4000, progressBar: true, showMethod: 'slideDown', hideMethod: 'slideUp' });
    }
    else if (data.success == 1) {
        toastr.error(data.message, data.title, { extendedTimeOut: true, timeOut: 4000, progressBar: true, showMethod: 'slideDown', hideMethod: 'slideUp' });
    }
    else if (data.success == 2) {
        toastr.info(data.message, data.title, { extendedTimeOut: true, timeOut: 7000, progressBar: true, showMethod: 'slideDown', hideMethod: 'slideUp' });
    }
    else if (data.success == 3) {
        toastr.warning(data.message, data.title, { extendedTimeOut: true, timeOut: 7000, progressBar: true, showMethod: 'slideDown', hideMethod: 'slideUp' });
    }
}