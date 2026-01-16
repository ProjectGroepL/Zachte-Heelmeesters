<script setup lang="ts">
import { CheckIcon, ChevronsUpDownIcon } from 'lucide-vue-next'
import { computed, ref } from 'vue'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'

export interface ComboboxOption {
  value: string | number
  label: string
  disabled?: boolean
}

interface ComboboxProps {
  modelValue?: string
  options?: ComboboxOption[]
  placeholder?: string
  searchPlaceholder?: string
  emptyMessage?: string
  disabled?: boolean
  class?: string
}

const props = withDefaults(defineProps<ComboboxProps>(), {
  modelValue: '',
  options: () => [],
  placeholder: 'Select option...',
  searchPlaceholder: 'Search...',
  emptyMessage: 'No option found.',
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const open = ref(false)

const selectedOption = computed(() =>
  props.options.find(option => option.value === props.modelValue),
)

function selectOption(selectedValue: string) {
  const newValue = selectedValue === props.modelValue ? '' : selectedValue
  emit('update:modelValue', newValue)
  open.value = false
}
</script>

<template>
  <Popover v-model:open="open">
    <PopoverTrigger as-child>
      <Button variant="outline" role="combobox" :aria-expanded="open" :disabled="disabled"
        :class="cn('w-full justify-between', props.class)">
        {{ selectedOption?.label || placeholder }}
        <ChevronsUpDownIcon class="ml-2 h-4 w-4 shrink-0 opacity-50" />
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-full p-0">
      <Command>
        <CommandInput class="h-9 focus-visible:outline-0!" :placeholder="searchPlaceholder" />
        <CommandList>
          <CommandEmpty class="text-foreground overflow-hidden p-1">
            <div
              class="data-[highlighted]:bg-accent data-[highlighted]:text-accent-foreground [&_svg:not([class*='text-'])]:text-muted-foreground relative flex cursor-default items-center gap-2 rounded-sm px-2 py-1.5 text-sm outline-hidden select-none pointer-events-none opacity-50 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4">
              {{ emptyMessage }}
            </div>
          </CommandEmpty>
          <CommandGroup>
            <CommandItem v-if="options.length > 0" v-for="option in options" :key="option.value" :value="option.value"
              :disabled="option.disabled" @select="(ev) => {
                selectOption(ev.detail.value as string)
              }">
              {{ option.label }}
              <CheckIcon :class="cn(
                'ml-auto h-4 w-4',
                modelValue === option.value ? 'opacity-100' : 'opacity-0',
              )" />
            </CommandItem>
            <CommandItem v-else value="-1" disabled>
              {{ emptyMessage }}
            </CommandItem>
          </CommandGroup>
        </CommandList>
      </Command>
    </PopoverContent>
  </Popover>
</template>
