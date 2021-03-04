// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Start parameters, table sorts
function setupPage() {
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();

    $('.no-action').click(function (e) {
        e.preventDefault();
    })

    if (location.href.includes('TeacherTests') ||
        location.href.includes('StudentsList') ||
        location.href.includes('StudentTests') ||
        location.href.includes('TeachersList') ||
        location.href.includes('TeacherInfo')) {
        $('table').tablesorter({ headers: { 0: { sorter: false } } });
        $('.tablesorter-header-inner').click(function () {
            setTimeout(function () {
                let i = 1;
                $('th[scope="row"]').each((idx, el) => $(el).text(i++));
            }, 0);
        });
    }
    else {
        $('table').tablesorter();
    }
}
setupPage();

// Result table
$(function () {

    let deleteResult = function (e) {
        let arr = $($(e.target).parents('tr').get(0)).attr('id').split('_');
        $('#resToDeleteSID').val(arr[1]);
        $('#resToDeleteTID').val(arr[0]);
    }

    $('.res-row-delete').click(deleteResult);

    // Result search
    if (location.pathname.includes('Result/ShowResults')) {
        let sMax = +$('#scoreEnd').val();
        let mMax = +$('#markEnd').val();

        let sFrom = 0;
        let sTo = sMax;
        let mFrom = 0;
        let mTo = mMax;

        $('input[name="searchType"]').click(function (e) {
            if ($(e.target).attr('id') == 'searchRadio1') {
                studentsOnly = true;
                groupsOnly = false;
            }
            else if ($(e.target).attr('id') == 'searchRadio2') {
                groupsOnly = true;
                studentsOnly = false;
            }
            else {
                groupsOnly = true;
                studentsOnly = true;
            }
        })

        $('#scoreStart').focusout(function (e) {
            let val = +$(e.target).val();
            if (!val || val <= 0) {
                $(e.target).val(0);
                sFrom = 0;
            }
            else if (val > sMax) {
                $(e.target).val(sMax);
                sFrom = sMax;
            }
            else if (val > sTo) {
                $(e.target).val(sTo);
                sFrom = sTo;
            }
            else {
                sFrom = val;
            }
        })
        $('#scoreEnd').focusout(function (e) {
            let val = +$(e.target).val();
            if (!val || val < sFrom) {
                $(e.target).val(sFrom);
                sTo = sFrom;
            }
            else if (val > sMax) {
                $(e.target).val(sMax);
                sTo = sMax;
            }
            else {
                sTo = val;
            }
        })
        $('#markStart').focusout(function (e) {
            let val = +$(e.target).val();
            if (!val || val <= 0) {
                $(e.target).val(0);
                mFrom = 0;
            }
            else if (val > mMax) {
                $(e.target).val(mMax);
                mFrom = mMax;
            }
            else if (val > mTo) {
                $(e.target).val(mTo);
                mFrom = mTo;
            }
            else {
                mFrom = val;
            }
        })
        $('#markEnd').focusout(function (e) {
            let val = +$(e.target).val();
            if (!val || val < mFrom) {
                $(e.target).val(mFrom);
                mTo = mFrom;
            }
            else if (val > mMax) {
                $(e.target).val(mMax);
                mTo = mMax;
            }
            else {
                mTo = val;
            }
        })


        search = function () {
            let searchParams = new URLSearchParams(window.location.search);
            let searchQuery = $('#searchInput').val();
            let studentsOnly = false, groupsOnly = false;
            if ($('input[name="searchType"]:checked').attr('id') == 'searchRadio1')
                studentsOnly = true;
            else
                groupsOnly = true;

            $.get(
                '/Result/ShowResults',
                {
                    testId: searchParams.get("testId"),
                    sq: searchQuery,
                    sOnly: studentsOnly,
                    gOnly: groupsOnly,
                    sFrom: sFrom,
                    sEnd: sTo,
                    mFrom: mFrom,
                    mEnd: mTo
                },
                function (data) {
                    data = $(data).find('.result-table');
                    if ($(data).find('.res-el').length == 0) {
                        $('.result-table').remove();
                        $('#noSearchResults').removeAttr('hidden');
                    }
                    else {
                        $('#noSearchResults').attr('hidden', 'hidden');
                        $('.result-table').remove();
                        $(data).insertAfter($('#tablePlace'));
                        $(data).find('.res-row-delete').click(deleteResult);
                        setupPage();
                    }
                }
            )
        }

        toDo = function (e) {
            if (e.key == 'Enter') {
                search();
            }
        }

        $('#searchInput').keydown(toDo);
        $('#searchBtn').click(search);

        $('#printResults').click(function (e) {
            e.preventDefault();
            window.open(`/Result/PrintResults?testId=${location.search.split('=')[1]}` +
                `&sq=${searchQuery}&sOnly=${studentsOnly}&gOnly=${groupsOnly}&sFrom=${sFrom}` +
                `&sEnd=${sTo}&mFrom=${mFrom}&mEnd=${mTo}`, '_blank');
        })
    }
})

// Test table
$(function () {
    let rowCanSelect = true;

    // Handlers

    let startTest = function (e) {
        e.preventDefault();
        let disabled = $(e.target).attr('disabled');
        let id = $('#testId').val();
        if (!disabled) {
            location.assign('/Test/PlayTest?id=' + id);
        }
    };

    let testInfo = function (e) {
        if (location.pathname.includes('Test/StudentTests')) {
            let parent = $(e.target).parents('.test-el').get(0);
            let id = $(parent).attr('id');
            location.assign('/Test/StudentTestInfo?id=' + id);
        }
        else if (location.pathname.includes('/Teacher/TeacherInfo')) {
            let parent = $(e.target).parents('.test-el').get(0);
            let id = $(parent).attr('id');
            if (rowCanSelect) {
                location.assign('/Test/TeacherTestInfo?id=' + id);
            }
            else
                rowCanSelect = true;
        }
        else {
            let parent = $(e.target).parents('.test-el').get(0);
            let id = $(parent).attr('id');
            if (rowCanSelect) {
                location.assign('TeacherTestInfo?id=' + id);
            }
            else
                rowCanSelect = true;
        }
    };

    let canSelect = function (e) {
        rowCanSelect = false;
    }

    let deleteRow = function (e) {
        e.preventDefault();
        let parent = $(e.target).parents('tr').get(0);
        $('#testToDelete').attr('value', $(parent).attr('id'));
        rowCanSelect = false;
    }

    $('.test-row-delete').click(deleteRow);
    $('.test-row-edit').click(canSelect);
    $('.test-el').click(testInfo);
    $('#startTestBtn').click(startTest);

    // Teacher tests search
    if (location.pathname.includes('Test/TeacherTests')) {
        let testSearch = function () {
            let searchQuery = $('#searchInput').val();


            let disciplinesOnly = false;
            let topicsOnly = false;
            let id = $('input[name="searchType"]:checked').attr('id');
            if (id == 'searchRadio1') {
                disciplinesOnly = true;
                topicsOnly = false;
            }
            else if (id == 'searchRadio2') {
                topicsOnly = true;
                disciplinesOnly = false;
            }
            else {
                topicsOnly = true;
                disciplinesOnly = true;
            }

            let hasResultOnly = $('#onlyResultCheck:checked').length ? true : false;

            $.get(
                '/Test/TeacherTests',
                {
                    sq: searchQuery,
                    dOnly: disciplinesOnly,
                    tOnly: topicsOnly,
                    rOnly: hasResultOnly
                },
                function (data) {
                    data = $(data).find('#testsTable');
                    if ($(data).find('.test-el').length == 0) {
                        $('#testsTable').remove();
                        $('#noSearchResults').removeAttr('hidden');
                    }
                    else {
                        $('#noSearchResults').attr('hidden', 'hidden');
                        $('#testsTable').remove();

                        $(data).find('.test-row-delete').click(deleteRow);
                        $(data).find('.test-row-edit').click(canSelect);
                        $(data).insertAfter($('#tablePlace'));
                        $(data).find('.test-el').click(testInfo);
                        setupPage();
                    }
                }
            )
        }

        toDo = function (e) {
            if (e.key == 'Enter') {
                testSearch();
            }
        }

        $('#searchInput').keydown(toDo);
        $('#searchBtn').click(testSearch);
    }

    // Student tests search
    if (location.pathname.includes('Test/StudentTests')) {
        let testSearch = function () {
            let searchQuery = $('#searchInput').val();


            let disciplinesOnly = false;
            let topicsOnly = false;
            let id = $('input[name="searchType"]:checked').attr('id');
            if (id == 'searchRadio1') {
                disciplinesOnly = true;
                topicsOnly = false;
            }
            else if (id == 'searchRadio2') {
                topicsOnly = true;
                disciplinesOnly = false;
            }
            else {
                topicsOnly = true;
                disciplinesOnly = true;
            }

            let hasResultOnly = $('#onlyResultCheck:checked').length ? true : false;

            $.get(
                '/Test/StudentTests',
                {
                    sq: searchQuery,
                    dOnly: disciplinesOnly,
                    tOnly: topicsOnly,
                    rOnly: hasResultOnly
                },
                function (data) {
                    data = $(data).find('#testsTable');
                    if ($(data).find('.test-el').length == 0) {
                        $('#testsTable').remove();
                        $('#noSearchResults').removeAttr('hidden');

                    }
                    else {
                        $('#noSearchResults').attr('hidden', 'hidden');
                        $('#testsTable').remove();

                        $(data).insertAfter($('#tablePlace'));
                        $(data).find('.test-el').click(testInfo);
                        $(data).find('#startTestBtn').click(startTest);
                    }
                }
            )
        }

        toDo = function (e) {
            if (e.key == 'Enter') {
                testSearch();
            }
        }

        $('#searchInput').keydown(toDo);
        $('#searchBtn').click(testSearch);
    }
})

// Students table
$(function () {
    let studentInfo = function (e) {
        let id = +$(e.target).parents('tr').attr('id');
        location.assign('/Student/StudentInfo?id=' + id);
    }
    $('.student-el').click(studentInfo);

    // Students search
    if (location.pathname.includes('Student/StudentsList')) {
        let studentSearch = function () {
            let searchQuery = $('#searchInput').val();


            let fioOnly = false;
            let groupsOnly = false;
            let id = $('input[name="searchType"]:checked').attr('id');
            if (id == 'searchRadio1') {
                fioOnly = true;
                groupsOnly = false;
            }
            else if (id == 'searchRadio2') {
                fioOnly = false;
                groupsOnly = true;
            }

            let hasResultOnly = $('#onlyResultCheck:checked').length ? true : false;
            let fiveOnly = $('#onlyFive:checked').length ? true : false;
            let twoOnly = $('#onlyTwo:checked').length ? true : false;

            $.get(
                '/Student/StudentsList',
                {
                    sq: searchQuery,
                    fOnly: fioOnly,
                    gOnly: groupsOnly,
                    hOnly: hasResultOnly,
                    twOnly: twoOnly,
                    fvOnly: fiveOnly,
                },
                function (data) {
                    data = $(data).find('#studentsTable');
                    if ($(data).find('.student-el').length == 0) {
                        $('#studentsTable').remove();
                        $('#noSearchResults').removeAttr('hidden');
                    }
                    else {
                        $('#noSearchResults').attr('hidden', 'hidden');
                        $('#studentsTable').remove();
                        $(data).insertAfter($('#tablePlace'));
                        $(data).find('.student-el').click(studentInfo);
                        setupPage();
                    }
                }
            )
        }

        toDo = function (e) {
            if (e.key == 'Enter') {
                studentSearch();
            }
        }

        $('#searchInput').keydown(toDo);
        $('#searchBtn').click(studentSearch);
    }
})

// Teachers table
$(function () {
    let teacherInfo = function (e) {
        let id = +$(e.target).parents('tr').attr('id');
        location.assign('/Teacher/TeacherInfo?id=' + id);
    }
    $('.teacher-el').click(teacherInfo);

    // Teachers search
    if (location.pathname.includes('Teacher/TeachersList')) {
        let teacherSearch = function () {
            let searchQuery = $('#searchInput').val();


            let fioOnly = false;
            let depOnly = false;
            let posOnly = false;
            let id = $('input[name="searchType"]:checked').attr('id');
            if (id == 'searchRadio1') {
                fioOnly = true;
                depOnly = false;
                posOnly = false;
            }
            else if (id == 'searchRadio2') {
                fioOnly = false;
                depOnly = true;
                posOnly = false;
            }
            else if (id == 'searchRadio3') {
                fioOnly = false;
                depOnly = false;
                posOnly = true;
            }

            let a = $('#onlyHasTest:checked').length > 0;
            let b = $('#onlyMail:checked').length > 0;

            $.get(
                '/Teacher/TeachersList',
                {
                    sq: searchQuery,
                    fOnly: fioOnly,
                    gOnly: depOnly,
                    pOnly: posOnly,
                    tOnly: $('#onlyHasTest:checked').length > 0,
                    kOnly: $('#onlyMail:checked').length > 0
                },
                function (data) {
                    data = $(data).find('#teachersTable');
                    if ($(data).find('.teacher-el').length == 0) {
                        $('#teachersTable').remove();
                        $('#noSearchResults').removeAttr('hidden');
                    }
                    else {
                        $('#noSearchResults').attr('hidden', 'hidden');
                        $('#teachersTable').remove();
                        $(data).insertAfter($('#tablePlace'));
                        $(data).find('.teacher-el').click(teacherInfo);
                        setupPage();
                    }
                }
            )
        }

        toDo = function (e) {
            if (e.key == 'Enter') {
                teacherSearch();
            }
        }

        $('#searchInput').keydown(toDo);
        $('#searchBtn').click(teacherSearch);
    }
})

// Login forms
$(function () {
    let loginForm = $('#loginForm');
    if (loginForm) {
        let val;
        $('input[name="role"]').click(function (e) {
            val = e.target.value;
            let btn = $('#regBtn');
            $(btn).click((e) => {
                e.preventDefault();
                location.assign('/Account/Register');
            })

            setDisableAttr = function (value) {
                if (!value)
                    btn.removeAttr('disabled');
                else
                    btn.attr('disabled', 'disabled');
            }

            if (val != 2)
                setDisableAttr(true);
            else
                setDisableAttr(false);
        })

        // Add disciplines inputs
        $('.btn-add-disc').click(function (e) {
            e.preventDefault();
            let content = $('#discInputPattern').clone();
            let i = $(content).find('.disc-abbr');
            let idx = $('.disc-abbr').length - 1;

            i.attr('name', i.attr('name').replace('<i>', idx));
            content.removeAttr('hidden');
            content.removeAttr('id');
            content.insertAfter('#discInputPattern')

            // Remove disciplines inputs
            $('.btn-remove-disc').click(function (e) {
                e.preventDefault();
                $(e.target).parents('.row').get(0).remove();
            })
        })
    }
})

// Question form
$(function () {
    let table = $('.question-table');
    if (table) {
        changeBox = function (e) {
            let q = $(e.target).attr('name').substr(5);
            $('#correct' + q).attr('name', $(e.target).attr('id'));
        }

        deleteAnswer = function (e) {
            e.preventDefault();
            $(e.target).parent().parent().remove();
        }

        addAnswer = function (e) {
            e.preventDefault();
            let content = $('#quest-el-answ-pattern').clone();
            let i = +$(e.target).parent().parent().children('th').text();
            let j = +$(e.target).parent().find('.quest-el-answ').length;

            $(content).html($(content).html().replace(/<i>/g, i - 1));
            $(content).html($(content).html().replace(/<j>/g, j));

            content.removeAttr('hidden');
            content.removeAttr('id');
            $(content).insertBefore($(e.target));

            content.find('.del-answ').click(deleteAnswer);
            content.find('.quest-el-check').click(changeBox);
        }

        deleteQuest = function (e) {
            e.preventDefault();
            $(e.target).parent().parent()
                .parent().parent().remove();
        }

        $('.btn-add-answ').click(addAnswer);
        $('.del-answ').click(deleteAnswer);
        $('.quest-el-check').click(changeBox);
        $('.del-quest').click(deleteQuest);

        $('.btn-add-quest').click(function (e) {
            e.preventDefault();
            let pattern = $('#questPattern');
            let content = pattern.clone();
            let i = $('.quest-el').length;

            content.removeAttr('hidden');
            content.removeAttr('id');
            content.children('th').text(i);
            content.html(content.html().replace(/<i>/g, i - 1));

            $(content).insertBefore(pattern);

            content.find('.del-quest').click(deleteQuest);
            content.find('.quest-el-check').click(changeBox);
            content.find('.btn-add-answ').click(addAnswer);
        })
    }
})

// Test info page
$(function () {
    let id = location.search.split('=')[1];

    $('#results').click(function (e) {
        e.preventDefault();
        let maxScore = $('#maxScore').text();
        let maxMark = $('#maxMark').text();
        location.href = `/Result/ShowResults?testId=${id}&sOnly=true&sEnd=${maxScore}&mEnd=${maxMark}`;
    })

    $('#edit').click(function (e) {
        e.preventDefault();
        location.href = '/Test/EditTest?id=' + id;
    })

    $('#print').click(function () {
        window.print();
    })
})

// Teacher info page
$(function () {
    let id = location.search.split('=')[1];

})

// Play test page
$(function () {
    if (location.pathname.includes('Test/PlayTest')) {
        let timerArr = $('#timer').text().split(':');
        let limit = +timerArr[0] * 60 + +timerArr[1];
        let sec = limit;
        let rogue = true;

        setInterval(function () {
            sec -= 1;
            if (sec == 0) {
                $('#endTest').trigger('click');
            }
            if (sec >= 0) {

                let min = parseInt(sec / 60);
                if (min.toString().length == 1) {
                    min = '0' + min.toString();
                }
                let s = sec % 60;
                if (s.toString().length == 1) {
                    s = '0' + s.toString();
                }
                $('#timer').text(`${min}:${s}`);
            }
        }, 1000)

        $('#q1').removeAttr('hidden');

        $('.btn-next').click(function (e) {
            e.preventDefault();
            let id = $($('.question-block:not(.question-block:hidden)').get(0)).attr('id');
            id = id.split('q')[1];
            let next = +id + 1;

            $('#q' + id).attr('hidden', 'hidden');
            $('#q' + next).removeAttr('hidden');
        })

        $('.btn-back').click(function (e) {
            e.preventDefault();
            let id = $($('.question-block:not(.question-block:hidden)').get(0)).attr('id');
            id = id.split('q')[1];
            let prev = +id - 1;

            $('#q' + id).attr('hidden', 'hidden');
            $('#q' + prev).removeAttr('hidden');
        })

        $('input[type="radio"]').change(function (e) {
            let arr = $(e.target).attr('id').split('a');
            let i = arr[0];
            let j = arr[1];

            let ida = `#trueAnswer_${i}`;
            let answInput = $(ida);

            answInput.removeAttr('name');
            answInput.attr('name',`Questions[${i}].Answers[${j}].IsTrue`);
        })

        $('#endTest').click(function () {
            $('#time').val(limit - sec);
            rogue = false;
        })

        //let searchParams = new URLSearchParams(window.location.search);
        //let id = searchParams.get('id');
        //$(window).on('beforeunload', function () {
        //    $.post('/Test/PlayTest', { id: id }, function (data) { });
        //});
    }
})

// Check test page
$(function () {
    $('#exit').click(function () {
        location.assign('/Test/StudentTests');
    })
})


//////////////////////////////////////////////////
let labels = [];
for (let i = 1; i <= +$('#maxScore').text(); i++) {
    labels[i - 1] = i;
}
let data = $('#data').text();
data.length = data.length - 1;
data = data.split('.');
let dataArr = [];
for (let i = 0; i < data.length - 1; i++) {
    dataArr[i] = +(data[i].replace(',', '.'));
}

var ctx = document.getElementById('myChart').getContext('2d');
var myChart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: labels,
        datasets: [{
            label: 'Сложность',
            data: dataArr,
            borderColor: "#ff6384",
            fill: false,
            pointRadius: 5
            //borderWidth: 1
        }]
    },
    options: {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                },
                scaleLabel: {
                    display: true,
                    labelString: "Сложность"
                },
            }],
            xAxes: [{
                scaleLabel: {
                    display: true,
                    labelString: "№ Вопроса"
                },
            }]
        }
    }
});

///////////////////////////////////////////////////////
let ltext = $('#datax').text();
ltext.length -= 1;
ltext = ltext.split(',');
let larr = [];
for (let i = 0; i < ltext.length - 1; i++) {
    larr[i] = +ltext[i];
}

data = $('#datay').text();
data.length = data.length - 1;
data = data.split(',');
dataArr = [];
for (let i = 0; i < data.length - 1; i++) {
    dataArr[i] = +data[i];
}

let fn = [];
for (let i = 0; i < dataArr.length; i++) {
    fn[i] = {
        x: larr[i],
        y: dataArr[i],
        r: 5
    }
}

ctx = document.getElementById('myChart1').getContext('2d');
myChart = new Chart(ctx, {
    type: 'bubble',
    data: {
        datasets: [
            {
                data: fn,
                backgroundColor: "#ff6384",
                label: "Результат"
            }
        ]
    },
    options: {
       scales: {
            yAxes: [{
                scaleLabel: {
                    display: true,
                    labelString: "Набранные баллы"
                },
                ticks: {
                    beginAtZero: true
                }
            }],
            xAxes: [{
                scaleLabel: {
                    display: true,
                    labelString: "Время, c"
                },
                ticks: {
                    beginAtZero: true
                }
            }]
        }
    }
});