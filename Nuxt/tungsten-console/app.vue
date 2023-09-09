<script setup lang="ts">
    import {ref} from 'vue'

    const logsUrl: string = "http://localhost:8080/api/logs";

    const input = ref<string>("");
    const intervalId = ref<number>(-1);

    const logs = ref<Log[]>([]);

    interface NewLogsResponse {
        logs: Log[];
    }

    interface Log {
        logString: string;
        stackTrace: string;
        logType: string;
        customColor: boolean;
        // textColor: Color;
        // bgColor: Color;
    }

    onMounted(() => {
        // start fetching logs
        intervalId.value = setInterval(() => { fetchNewLogs(); }, 1000) as unknown as number;
    });

    async function fetchNewLogs() {
        fetchData<NewLogsResponse>(logsUrl).then((data) => {
            // add new logs to the list
            logs.value.push(...data.logs);
        }).catch((error) => {
            console.log(error);
        });
    }

    async function fetchData<T>(url: string): Promise<T> {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        return await response.json() as T;
    }

    function submitCommand() {
        console.log(input.value);
        input.value = "";
    }
</script>

<template>
    <div class="console">
        <div class="log">
            <div class="log-entry" v-for="(log, index) in logs" :key="index">
                {{ log.logString }}
            </div>

        </div>

        <div class="input">
            <div class="command">
                <UInput icon="i-heroicons-code-bracket-20-solid" size="sm" color="white" :trailing="false" @keyup.enter="submitCommand" v-model="input"/>
            </div>
            <div class="search">
                <UInput icon="i-heroicons-magnifying-glass-20-solid" size="sm" color="white" :trailing="false"/>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .console {
        width: 100vw;
        height: 100vh;
        display: flex;
        flex-direction: column;
        margin: 0;
    }

    .log {
        flex: 1;
        overflow-y: auto;
    }

    .log-entry {
        padding: 16px;
    }

    .input {
        width: 100%;
        padding: 16px;
        display: flex;
        flex-direction: row;
    }
    
    .command {
        flex: 1;
    }

    .search {
        margin-left: 16px;
        width: 300px;
    }
</style>