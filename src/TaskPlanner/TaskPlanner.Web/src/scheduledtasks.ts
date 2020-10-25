import * as ko from "knockout";
import 'bootstrap/dist/css/bootstrap.min.css';
import './style.css';

class AppViewModel {
    public scheduledTasks: KnockoutObservableArray<ScheduledTaskViewModel> = ko.observableArray();
    public users = ko.observableArray<{ id: number, name: string }>();

    constructor() {
        this.init();
    }

    async init() {
        let result = await fetch('api/User/');
        this.users(await result.json());
        this.users.unshift({ id: -1, name: '-' });

        await this.loadList();
    }

    async loadList() {
        let result = await fetch('api/ScheduledTask/Todo');
        let result2 = await fetch('api/ScheduledTask/GetAll');
        this.scheduledTasks((<any[]>await result.json()).map(x => new ScheduledTaskViewModel(this, x)));
    }
}

class ScheduledTaskViewModel {
    constructor(private parent: AppViewModel, model: any) {
        this.id = model.id;
        this.date = model.date;
        this.name = model.name;
        this.description = model.description;
        this.assignedUser = ko.observable<number>(model.assignedUser || -1);
        this.assignedUser.subscribe(() => this.setAssignedUser());
    }

    public async setDone() {
        this.loading(true);
        await fetch(`api/ScheduledTask/${this.id}/SetDone`, { method: 'POST' });
        await this.parent.loadList();
        this.loading(false);
    }

    private async setAssignedUser() {
        this.loading(true);
        await fetch(`api/ScheduledTask/${this.id}/SetAssignedUser/${this.assignedUser()}`, { method: 'POST' });
        await this.parent.loadList();
        this.loading(false);
    }

    id: number;
    date: string;
    name: string;
    description: string;
    assignedUser: KnockoutObservable<number>;
    loading = ko.observable(false);
}

function init() {
    ko.applyBindings(new AppViewModel(), document.getElementById("main"));
}

window.onload = init;